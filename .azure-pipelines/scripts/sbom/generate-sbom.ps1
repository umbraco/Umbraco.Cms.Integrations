#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates a CycloneDX SBOM per package with umbraco-sbom, checks license policy, and
    optionally uploads each one to Dependency-Track.

.DESCRIPTION
    Ported from Umbraco.AI (.azure-pipelines/scripts/sbom/generate-sbom.ps1), adapted to this
    repo's layout: each package is src/<package>/ holding its own version.json
    (Nerdbank.GitVersioning) and <package>.csproj, and its backoffice client, if it has one, is an
    npm workspace of the root package.json at src/<package>/Client.

    For each package:
    1. Works out the package version with nbgv (the version that ships, e.g. 7.0.2).
    2. Restores and scans the package's .csproj. Only the shipped project is scanned, so the test
       projects and examples/ aren't in the SBOM or subject to the license policy. The .csproj is
       passed rather than the folder: the folder also holds Client/, which umbraco-sbom would
       reject as a mixed-ecosystem path (exit 41).
    3. Scans the package's npm workspace from a slice of the root lockfile holding only its
       runtime dependencies (see prune-npm-lockfile.js), then merges everything into one SBOM.
    4. Fails the package on a license policy violation (umbraco-sbom exit 50).
    5. With -Upload, posts the SBOM to Dependency-Track as one project per package major
       (e.g. "Umbraco.Cms.Integrations.Search.Algolia v7") with the full version (e.g. 7.0.2).
       isLatest is set when the version is stable and at least as high as the newest stable
       version of the same major on NuGet.org, so each major's project marks its newest stable
       release. An upload failure is a warning, not a failure: it must not block a release.

.PARAMETER Products
    Comma-separated package folder names under src/ to generate SBOMs for.

.PARAMETER OutputDir
    Folder the <package>-bom.xml files are written to.

.PARAMETER Upload
    Upload each SBOM to Dependency-Track. Reads DT_BASE_URL and DT_API_KEY from the environment.

.PARAMETER PolicyAllowFile
    Per-package license exceptions passed to umbraco-sbom --policy-allow-file, if the file exists.

.EXAMPLE
    ./generate-sbom.ps1 -Products "Umbraco.Cms.Integrations.Search.Algolia" -OutputDir ./sbom
#>

param(
    # Not mandatory: an empty list (nothing changed) is valid and generates nothing.
    [string]$Products = "",

    [Parameter(Mandatory)]
    [string]$OutputDir,

    [switch]$Upload,

    [string]$PolicyAllowFile = ".sbom-policy.json",

    [string]$RootPath = (Get-Location).Path
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$PolicyViolationExitCode = 50
$UnknownLicensesExitCode = 10

$workDir = Join-Path ([System.IO.Path]::GetTempPath()) "sbom-$([guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Force -Path $workDir, $OutputDir | Out-Null
$OutputDir = (Resolve-Path $OutputDir).Path

$policyArgs = @()
$policyFilePath = Join-Path $RootPath $PolicyAllowFile
if (Test-Path $policyFilePath) {
    Write-Host "Using license policy exceptions from $PolicyAllowFile"
    $policyArgs = @("--policy-allow-file", $policyFilePath)
}

$rootLockfile = Join-Path $RootPath "package-lock.json"
$workspaces = @()
$rootPackageJson = Join-Path $RootPath "package.json"
if (Test-Path $rootPackageJson) {
    $rootPackage = Get-Content $rootPackageJson -Raw | ConvertFrom-Json
    if ($rootPackage.PSObject.Properties.Name -contains "workspaces") {
        $workspaces = @($rootPackage.workspaces)
    }
}

function Invoke-WithRetry {
    param([scriptblock]$Action, [string]$Description, [int]$MaxAttempts = 3)

    $delay = 15
    for ($attempt = 1; ; $attempt++) {
        try {
            return & $Action
        }
        catch {
            if ($attempt -ge $MaxAttempts) { throw }
            Write-Host "##[warning]$Description failed (attempt $attempt/$MaxAttempts): $($_.Exception.Message). Retrying in ${delay}s..."
            Start-Sleep -Seconds $delay
            $delay *= 2
        }
    }
}

function Get-ProductVersion {
    param([string]$ProductPath)

    Push-Location $ProductPath
    try {
        $version = nbgv get-version -v NuGetPackageVersion
        if ($LASTEXITCODE -ne 0 -or -not $version) { throw "nbgv get-version failed in $ProductPath" }
        return $version.Trim()
    }
    finally {
        Pop-Location
    }
}

# Runs umbraco-sbom and returns $true on success. Unknown licenses (exit 10) still write a
# valid SBOM and pass the default policy, so they count as success.
function Invoke-UmbracoSbom {
    param([string]$InputPath, [string]$OutputFile)

    umbraco-sbom $InputPath --output-file $OutputFile @policyArgs | Out-Host
    switch ($LASTEXITCODE) {
        0 { return $true }
        $UnknownLicensesExitCode { return $true }
        $PolicyViolationExitCode {
            Write-Host "##[error]License policy violation in $InputPath - see the violated components above"
            return $false
        }
        default {
            Write-Host "##[error]umbraco-sbom failed for $InputPath with exit code $LASTEXITCODE"
            return $false
        }
    }
}

function New-NuGetSbom {
    param([string]$Product, [string]$ProductPath, [string]$OutputFile)

    $project = Join-Path $ProductPath "$Product.csproj"
    if (-not (Test-Path $project)) {
        Write-Host "No $Product.csproj in $ProductPath"
        return $null
    }

    Invoke-WithRetry -Description "dotnet restore $Product" -Action {
        dotnet restore $project | Out-Host
        if ($LASTEXITCODE -ne 0) { throw "dotnet restore exited with $LASTEXITCODE" }
    }

    if (-not (Invoke-UmbracoSbom -InputPath $project -OutputFile $OutputFile)) { return $false }
    return $OutputFile
}

function New-NpmSboms {
    param([string]$Product, [string]$OutputFilePrefix)

    # -like is case-insensitive, matching detect-changes.ps1's tolerance of folder casing drift.
    $productWorkspaces = @($workspaces | Where-Object { $_.Replace('\', '/') -like "src/$Product/*" })
    if ($productWorkspaces.Count -eq 0) { return @() }

    $files = @()
    $index = 0
    foreach ($workspace in $productWorkspaces) {
        $index++
        $sliceDir = Join-Path $workDir "$Product-npm-$index"
        node (Join-Path $PSScriptRoot "prune-npm-lockfile.js") $rootLockfile $workspace $sliceDir | Out-Host
        if ($LASTEXITCODE -ne 0) {
            Write-Host "##[error]Could not build the npm lockfile for $workspace"
            return $false
        }

        $outputFile = "$OutputFilePrefix-npm-$index.xml"
        if (-not (Invoke-UmbracoSbom -InputPath $sliceDir -OutputFile $outputFile)) { return $false }
        $files += $outputFile
    }
    return $files
}

# Merges the component lists of several CycloneDX XML files into the first, skipping any
# component already present (same bom-ref), and names the BOM after the package.
# umbraco-sbom output has no dependency graph, so components are all there is to merge.
function Merge-Sboms {
    param([string[]]$Files, [string]$Product, [string]$Version, [string]$OutputFile)

    [xml]$merged = Get-Content $Files[0] -Raw
    $bomNs = $merged.DocumentElement.NamespaceURI
    $ns = New-Object System.Xml.XmlNamespaceManager($merged.NameTable)
    $ns.AddNamespace("bom", $bomNs)

    $target = $merged.SelectSingleNode("/bom:bom/bom:components", $ns)
    if ($null -eq $target) {
        $target = $merged.DocumentElement.AppendChild($merged.CreateElement("components", $bomNs))
    }
    $seen = [System.Collections.Generic.HashSet[string]]::new()
    foreach ($component in $target.SelectNodes("bom:component", $ns)) {
        [void]$seen.Add($component.GetAttribute("bom-ref"))
    }

    foreach ($file in ($Files | Select-Object -Skip 1)) {
        [xml]$other = Get-Content $file -Raw
        foreach ($component in $other.SelectNodes("/bom:bom/bom:components/bom:component", $ns)) {
            if ($seen.Add($component.GetAttribute("bom-ref"))) {
                [void]$target.AppendChild($merged.ImportNode($component, $true))
            }
        }
    }

    # The BOM describes the package, not the project or lockfile slice scanned.
    $metadata = $merged.SelectSingleNode("/bom:bom/bom:metadata", $ns)
    if ($null -ne $metadata -and $null -eq $metadata.SelectSingleNode("bom:component", $ns)) {
        $root = $merged.CreateElement("component", $bomNs)
        $root.SetAttribute("type", "application")
        $root.SetAttribute("bom-ref", "$Product@$Version")
        foreach ($field in @(@("name", $Product), @("version", $Version))) {
            $node = $merged.CreateElement($field[0], $bomNs)
            $node.InnerText = $field[1]
            [void]$root.AppendChild($node)
        }
        # Schema order puts component before properties.
        $properties = $metadata.SelectSingleNode("bom:properties", $ns)
        if ($null -ne $properties) {
            [void]$metadata.InsertBefore($root, $properties)
        }
        else {
            [void]$metadata.AppendChild($root)
        }
    }

    $merged.Save($OutputFile)
}

# Newest stable release of its major: stable, and not below any stable release of the same major
# on NuGet.org. Each major is its own Dependency-Track project, so each has its own latest.
# The package folder name is the NuGet package ID for every package in this repo.
function Test-IsLatest {
    param([string]$Product, [string]$Version)

    $current = [System.Management.Automation.SemanticVersion]$Version
    if ($current.PreReleaseLabel) { return $false }

    $uri = "https://api.nuget.org/v3-flatcontainer/$($Product.ToLowerInvariant())/index.json"
    $index = Invoke-WithRetry -Description "NuGet lookup for $Product" -Action {
        $response = Invoke-RestMethod -Uri $uri -SkipHttpErrorCheck -StatusCodeVariable status
        if ($status -eq 404) { return $null } # never published
        if ($status -ne 200) { throw "NuGet.org returned HTTP $status" }
        return $response
    }
    if ($null -eq $index) { return $true } # first stable release of a new package or major

    $published = @($index.versions |
        ForEach-Object {
            $parsed = $null
            if ([System.Management.Automation.SemanticVersion]::TryParse($_, [ref]$parsed)) { $parsed }
        } |
        Where-Object { -not $_.PreReleaseLabel -and $_.Major -eq $current.Major } |
        Sort-Object -Descending)

    return $published.Count -eq 0 -or $current -ge $published[0]
}

function Get-DependencyTrackProjectName {
    param([string]$Product, [string]$Version)

    $major = ([System.Management.Automation.SemanticVersion]$Version).Major
    return "$Product v$major"
}

function Send-Sbom {
    param([string]$Product, [string]$Version, [bool]$IsLatest, [string]$SbomFile)

    $baseUrl = $env:DT_BASE_URL
    if (-not $baseUrl -or -not $env:DT_API_KEY) {
        Write-Host "##[warning]DT_BASE_URL or DT_API_KEY not set - skipping upload of $Product"
        return $false
    }

    $form = @{
        autoCreate     = "true"
        projectName    = Get-DependencyTrackProjectName -Product $Product -Version $Version
        projectVersion = $Version
        isLatest       = $IsLatest.ToString().ToLowerInvariant()
        bom            = Get-Item $SbomFile
    }

    Invoke-WithRetry -Description "Dependency-Track upload of $Product" -Action {
        Invoke-RestMethod -Method Post -Uri "$($baseUrl.TrimEnd('/'))/api/v1/bom" `
            -Headers @{ "X-Api-Key" = $env:DT_API_KEY } -Form $form | Out-Null
    }
    return $true
}

$productList = @($Products -split "," | ForEach-Object { $_.Trim() } | Where-Object { $_ })
if ($productList.Count -eq 0) {
    Write-Host "No packages to generate SBOMs for"
    exit 0
}

$failed = @()
$uploadWarnings = @()

foreach ($product in $productList) {
    Write-Host ""
    Write-Host "##[section]$product"

    try {
        $productPath = Join-Path (Join-Path $RootPath "src") $product
        if (-not (Test-Path (Join-Path $productPath "version.json"))) {
            throw "version.json not found in $productPath"
        }

        $version = Get-ProductVersion -ProductPath $productPath
        Write-Host "Version: $version"

        $prefix = Join-Path $workDir $product
        $nuget = New-NuGetSbom -Product $product -ProductPath $productPath -OutputFile "$prefix-nuget.xml"
        $npm = New-NpmSboms -Product $product -OutputFilePrefix $prefix
        if ($nuget -eq $false -or $npm -eq $false) {
            $failed += $product
            continue
        }

        $files = @(@($nuget) + @($npm) | Where-Object { $_ })
        if ($files.Count -eq 0) {
            Write-Host "##[warning]$product has no .NET project or npm workspace - nothing to scan"
            continue
        }

        $sbomFile = Join-Path $OutputDir "$product-bom.xml"
        Merge-Sboms -Files $files -Product $product -Version $version -OutputFile $sbomFile
        Write-Host "SBOM written: $sbomFile"

        if ($Upload) {
            try {
                $isLatest = Test-IsLatest -Product $product -Version $version
                $projectName = Get-DependencyTrackProjectName -Product $product -Version $version
                Write-Host "Uploading to Dependency-Track as $projectName $version (isLatest: $isLatest)"
                if (-not (Send-Sbom -Product $product -Version $version -IsLatest $isLatest -SbomFile $sbomFile)) {
                    $uploadWarnings += $product
                }
            }
            catch {
                Write-Host "##[warning]Upload of $product failed: $($_.Exception.Message)"
                $uploadWarnings += $product
            }
        }
    }
    catch {
        Write-Host "##[error]$product failed: $($_.Exception.Message)"
        $failed += $product
    }
}

Remove-Item -Recurse -Force $workDir -ErrorAction SilentlyContinue

Write-Host ""
if ($uploadWarnings.Count -gt 0) {
    Write-Host "##vso[task.logissue type=warning]Dependency-Track upload failed for: $($uploadWarnings -join ', ')"
    Write-Host "##vso[task.complete result=SucceededWithIssues;]"
}
if ($failed.Count -gt 0) {
    Write-Host "##vso[task.logissue type=error]SBOM generation failed for: $($failed -join ', ')"
    exit 1
}
Write-Host "SBOMs generated for: $($productList -join ', ')"

# Exit explicitly: the PowerShell task otherwise reports the last native command's exit code,
# and umbraco-sbom exits 10 (unknown licenses) on success.
exit 0
