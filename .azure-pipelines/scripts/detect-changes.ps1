# ============================================================================
# Umbraco.Cms.Integrations change detection
# ============================================================================
# Works out which packages under src/ actually changed, and emits an Azure
# Pipelines matrix so only those get built and packed.
#
# The packages in this repo are independent siblings - none reference another -
# so there is deliberately no dependency graph and no build ordering here.
# A package is "a package" if it has both a .csproj and a version.json.
# ============================================================================

param(
    [string]$SourceBranch = $env:BUILD_SOURCEBRANCH,
    [string]$RootPath = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

# Root files that force every package to rebuild when they change.
$GlobalBuildFiles = @("azure-pipelines.yml", "global.json", "NuGet.config")

function Get-Products {
    <#
    .SYNOPSIS
    Discovers packable packages under src/.
    .DESCRIPTION
    A folder qualifies when it contains both <Name>.csproj and version.json.
    The version.json requirement keeps stale/legacy folders (and any bin/obj
    leftovers from other branches) out of the build.
    #>
    param([string]$RootPath)

    Write-Host "Discovering packages..." -ForegroundColor Cyan

    $srcPath = Join-Path $RootPath "src"
    $products = @{}

    Get-ChildItem -Path $srcPath -Directory | Where-Object {
        $_.Name -like "Umbraco.Cms.Integrations.*"
    } | ForEach-Object {
        $name = $_.Name
        $csproj = Join-Path $_.FullName "$name.csproj"
        $versionJson = Join-Path $_.FullName "version.json"

        if (-not (Test-Path $csproj)) { return }
        if (-not (Test-Path $versionJson)) {
            Write-Host "  ! Skipped: $name (no version.json)" -ForegroundColor DarkGray
            return
        }

        # A package builds client assets when it has a Client/package.json.
        $hasNpm = Test-Path (Join-Path $_.FullName "Client\package.json")

        $products[$name] = @{
            Name    = $name
            Path    = "src/$name"
            Project = "src/$name/$name.csproj"
            HasNpm  = $hasNpm
        }

        Write-Host "  + $name (client assets: $hasNpm)" -ForegroundColor Green
    }

    if ($products.Count -eq 0) {
        throw "No packages discovered under src/ - check that version.json files are present"
    }

    return $products
}

function Get-ComparisonBase {
    <#
    .SYNOPSIS
    Picks the commit to diff against for this build context.
    #>
    param([string]$SourceBranch)

    # Pull request: diff against the merge-base with the target branch.
    if ($env:SYSTEM_PULLREQUEST_TARGETBRANCH) {
        $target = $env:SYSTEM_PULLREQUEST_TARGETBRANCH -replace '^refs/heads/', ''
        Write-Host "  PR build, target branch: $target" -ForegroundColor Cyan

        $mergeBase = git merge-base "origin/$target" HEAD 2>&1
        if ($LASTEXITCODE -eq 0) { return $mergeBase.Trim() }

        Write-Host "  ! merge-base with origin/$target failed" -ForegroundColor Yellow
        return $null
    }

    # Integration branches: diff against the previous commit on the branch.
    if ($SourceBranch -match '^refs/heads/(main-v\d+|v\d+/dev)$') {
        Write-Host "  Integration branch build, comparing against HEAD~1" -ForegroundColor Cyan
        return "HEAD~1"
    }

    # Feature branch: diff against where it forked from its line's dev branch.
    $line = if ($SourceBranch -match '^refs/heads/(v\d+)/') { $Matches[1] } else { 'v18' }
    Write-Host "  Feature branch build, comparing against origin/$line/dev" -ForegroundColor Cyan

    $mergeBase = git merge-base "origin/$line/dev" HEAD 2>&1
    if ($LASTEXITCODE -eq 0) { return $mergeBase.Trim() }

    Write-Host "  ! merge-base with origin/$line/dev failed" -ForegroundColor Yellow
    return $null
}

function Get-ProductsAffectedByPackageProps {
    <#
    .SYNOPSIS
    Maps a Directory.Packages.props change to the packages that reference the
    packages whose versions actually moved.
    #>
    param(
        [string]$ComparisonBase,
        [hashtable]$Products
    )

    $diff = git diff "$ComparisonBase..HEAD" -- "Directory.Packages.props" 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "    Could not diff Directory.Packages.props - treating all as affected" -ForegroundColor Yellow
        return @($Products.Keys)
    }

    $changedPackages = @()
    foreach ($line in $diff) {
        if ($line -is [string] -and $line -match '^[+-].*<(?:PackageVersion|GlobalPackageReference)\s+Include="([^"]+)"') {
            if ($changedPackages -notcontains $Matches[1]) { $changedPackages += $Matches[1] }
        }
    }

    if ($changedPackages.Count -eq 0) {
        Write-Host "    No package version changes in diff" -ForegroundColor Gray
        return @()
    }

    Write-Host "    Changed package versions: $($changedPackages -join ', ')" -ForegroundColor Cyan

    # A GlobalPackageReference applies to every project, so treat it as global.
    $globalChange = $diff | Where-Object {
        $_ -is [string] -and $_ -match '^[+-].*<GlobalPackageReference'
    }
    if ($globalChange) {
        Write-Host "    GlobalPackageReference changed - affects all packages" -ForegroundColor Yellow
        return @($Products.Keys)
    }

    $affected = @()
    foreach ($name in $Products.Keys) {
        $content = Get-Content $Products[$name].Project -Raw -ErrorAction SilentlyContinue
        if (-not $content) { continue }

        foreach ($pkg in $changedPackages) {
            if ($content -match ('Include="' + [regex]::Escape($pkg) + '"')) {
                $affected += $name
                Write-Host "    * $name references $pkg" -ForegroundColor Green
                break
            }
        }
    }

    return $affected
}

function Get-ChangedProducts {
    param(
        [hashtable]$Products,
        [string]$SourceBranch
    )

    $changed = @{}
    $Products.Keys | ForEach-Object { $changed[$_] = $false }

    Write-Host ""
    Write-Host "Detecting changes..." -ForegroundColor Cyan

    $base = Get-ComparisonBase -SourceBranch $SourceBranch
    if (-not $base) {
        Write-Host "  No usable comparison base - building all packages" -ForegroundColor Yellow
        $Products.Keys | ForEach-Object { $changed[$_] = $true }
        return $changed
    }

    $changedFiles = git diff --name-only "$base" HEAD 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  git diff failed - building all packages" -ForegroundColor Yellow
        $Products.Keys | ForEach-Object { $changed[$_] = $true }
        return $changed
    }

    $changedFiles = @($changedFiles | Where-Object { $_ -is [string] })
    Write-Host "  Comparing $base..HEAD ($($changedFiles.Count) files changed)" -ForegroundColor Gray

    # Per-package folder changes.
    foreach ($file in $changedFiles) {
        foreach ($name in $Products.Keys) {
            if ($file.StartsWith($Products[$name].Path + "/")) {
                if (-not $changed[$name]) {
                    $changed[$name] = $true
                    Write-Host "  * $name changed ($file)" -ForegroundColor Green
                }
                break
            }
        }
    }

    # Shared build files.
    foreach ($file in $changedFiles) {
        if ($file -eq "Directory.Packages.props") {
            Write-Host "  Directory.Packages.props changed - tracing affected packages" -ForegroundColor Yellow
            foreach ($name in (Get-ProductsAffectedByPackageProps -ComparisonBase $base -Products $Products)) {
                $changed[$name] = $true
            }
        }
        elseif ($GlobalBuildFiles -contains $file -or $file -like ".azure-pipelines/*") {
            Write-Host "  $file changed - affects all packages" -ForegroundColor Yellow
            $Products.Keys | ForEach-Object { $changed[$_] = $true }
            break
        }
    }

    return $changed
}

function Write-PipelineVariables {
    param(
        [hashtable]$Products,
        [hashtable]$Changed
    )

    Write-Host ""
    Write-Host "Build plan:" -ForegroundColor Cyan

    $matrix = @{}
    foreach ($name in ($Products.Keys | Sort-Object)) {
        if ($Changed[$name]) {
            # Matrix keys cannot contain dots.
            $key = $name -replace '[.-]', '_'
            $matrix[$key] = @{
                name    = $Products[$name].Name
                path    = $Products[$name].Path
                project = $Products[$name].Project
                hasNpm  = $Products[$name].HasNpm.ToString().ToLower()
            }
            Write-Host "  BUILD $name" -ForegroundColor Green
        }
        else {
            Write-Host "  skip  $name" -ForegroundColor Gray
        }
    }

    $anyChanged = ($matrix.Count -gt 0).ToString().ToLower()
    $matrixJson = if ($matrix.Count -gt 0) { $matrix | ConvertTo-Json -Depth 3 -Compress } else { "{}" }

    Write-Host ""
    Write-Host "##vso[task.setvariable variable=AnyChanged;isOutput=true]$anyChanged"
    Write-Host "##vso[task.setvariable variable=BuildMatrix;isOutput=true]$matrixJson"
    Write-Host "  AnyChanged:  $anyChanged" -ForegroundColor Cyan
    Write-Host "  BuildMatrix: $matrixJson" -ForegroundColor Magenta
}

# ============================================================================
# Main
# ============================================================================

Write-Host "======================================="
Write-Host "Umbraco.Cms.Integrations change detection"
Write-Host "======================================="
Write-Host "Source branch: $SourceBranch"
Write-Host "Root path:     $RootPath"
Write-Host ""

$products = Get-Products -RootPath $RootPath
$changed = Get-ChangedProducts -Products $products -SourceBranch $SourceBranch
Write-PipelineVariables -Products $products -Changed $changed

Write-Host ""
Write-Host "Change detection complete" -ForegroundColor Green
