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
    [string]$RootPath = (Get-Location).Path,

    # Overrides the "last released state" ref that a release/hotfix build compares
    # against. Defaults to origin/main-v<N>, derived from the branch. Exists so the
    # release-selection logic can be exercised locally without pushing a branch.
    [string]$ReleaseCompareRef = ""
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

    # Map lowercased folder name -> the casing git actually tracks.
    # `git show <ref>:<path>` and `git diff -- <path>` are case-SENSITIVE even on
    # Windows, because they look paths up inside tree objects rather than on disk.
    # A working copy whose folder casing has drifted from the index (this repo has
    # `...GoogleSearchConsole.URLInspectionTool` in git but has been seen on disk as
    # `...UrlInspectionTool`) would otherwise make every git path lookup miss.
    $gitCasing = @{}
    git ls-tree -d --name-only HEAD src/ 2>$null | Where-Object { $_ -is [string] } | ForEach-Object {
        $gitName = Split-Path $_ -Leaf
        $gitCasing[$gitName.ToLower()] = $gitName
    }

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

        # A package builds client assets when it has a Client/package.json. That
        # folder is also an npm workspace of the root package.json, so capture its
        # workspace name - CI builds just that one with `npm run build -w <name>`.
        $clientManifest = Join-Path (Join-Path $_.FullName "Client") "package.json"
        $hasNpm = Test-Path $clientManifest
        $clientName = if ($hasNpm) { (Get-Content $clientManifest -Raw | ConvertFrom-Json).name } else { "" }

        # Prefer the casing git tracks; fall back to the disk name for a package
        # that is new and not yet committed.
        $gitName = $gitCasing[$name.ToLower()]
        if (-not $gitName) { $gitName = $name }

        $products[$gitName] = @{
            Name       = $gitName
            Path       = "src/$gitName"
            Project    = "src/$gitName/$gitName.csproj"
            HasNpm     = $hasNpm
            ClientName = $clientName
        }

        if ($gitName -ne $name) {
            Write-Host "  + $gitName (client assets: $hasNpm) [disk casing differs: $name]" -ForegroundColor Yellow
        }
        else {
            Write-Host "  + $gitName (client assets: $hasNpm)" -ForegroundColor Green
        }
    }

    if ($products.Count -eq 0) {
        throw "No packages discovered under src/ - check that version.json files are present"
    }

    return $products
}

function Test-IsReleaseBranch {
    param([string]$SourceBranch)
    return $SourceBranch -match '^refs/heads/v\d+/(release|hotfix)/'
}

function Get-ReleaseProducts {
    <#
    .SYNOPSIS
    On a release/hotfix branch, selects the packages that have a new version to
    publish - those whose version.json differs from the released state on main-v<N>.

    .DESCRIPTION
    This is what makes a release-manifest.json unnecessary. A release branch exists
    to publish specific packages, and what marks a package as "being released" is a
    bumped version.json. Comparing against main-v<N> (the last released state)
    therefore yields exactly the set to build and pack, and never picks up a package
    that merely has unreleased code changes.
    #>
    param(
        [hashtable]$Products,
        [string]$SourceBranch,
        [string]$CompareRef = ""
    )

    $changed = @{}
    $Products.Keys | ForEach-Object { $changed[$_] = $false }

    if ($CompareRef) {
        $releasedRef = $CompareRef
    }
    else {
        $line = if ($SourceBranch -match '^refs/heads/(v\d+)/') { $Matches[1] } else { 'v18' }
        $releasedRef = "origin/main-$line"
    }

    Write-Host "  Release branch build - selecting packages bumped vs $releasedRef" -ForegroundColor Cyan

    foreach ($name in ($Products.Keys | Sort-Object)) {
        $versionFile = "$($Products[$name].Path)/version.json"

        $releasedJson = git show "${releasedRef}:${versionFile}" 2>$null
        if ($LASTEXITCODE -ne 0) {
            # No version.json on the released ref yet - new to this line, so release it.
            $changed[$name] = $true
            Write-Host "  * $name is new on $releasedRef - including" -ForegroundColor Green
            continue
        }

        $releasedVersion = ($releasedJson | ConvertFrom-Json).version
        $currentVersion = (Get-Content $versionFile -Raw | ConvertFrom-Json).version

        if ($releasedVersion -ne $currentVersion) {
            $changed[$name] = $true
            Write-Host "  * $name $releasedVersion -> $currentVersion" -ForegroundColor Green
        }
        else {
            Write-Host "    $name unchanged at $currentVersion" -ForegroundColor Gray
        }
    }

    if (-not ($changed.Values | Where-Object { $_ })) {
        throw "No package has a bumped version.json compared with $releasedRef. A release branch must bump at least one package - run /release-management."
    }

    return $changed
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

    # A release/hotfix branch publishes whatever it bumped, not whatever it touched.
    if (Test-IsReleaseBranch -SourceBranch $SourceBranch) {
        return Get-ReleaseProducts -Products $Products -SourceBranch $SourceBranch -CompareRef $ReleaseCompareRef
    }

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
            # Every key here becomes a job VARIABLE, and job variables are exported
            # as environment variables. A key called `path` therefore overwrites
            # PATH with a relative source folder, and the job can no longer find
            # any executable - pwsh, bash, powershell, dotnet, all of them.
            # That is what killed every Pack job in builds 279692-279725 while
            # DetectChanges and Test, which have no matrix, ran fine.
            # Keep these prefixed; do not shorten them back.
            $matrix[$key] = @{
                productName = $Products[$name].Name
                productPath = $Products[$name].Path
                projectPath = $Products[$name].Project
                hasNpm      = $Products[$name].HasNpm.ToString().ToLower()
                clientName  = $Products[$name].ClientName
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
