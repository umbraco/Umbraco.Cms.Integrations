# ============================================================================
# Umbraco.Cms.Integrations change detection
# ============================================================================
# Works out which packages under src/ actually changed, and emits an Azure
# Pipelines matrix so only those get built and packed.
#
# The packages in this repo are independent siblings - none reference another -
# so there is deliberately no dependency graph and no build ordering here.
# A package is "a package" if it has both a .csproj and a version.json.
#
# On a release or hotfix branch there are two layers, because "what changed" and
# "what are we publishing" are different questions:
#
#   1. Detection compares each package against the released state on main-v<N>,
#      ignoring release bookkeeping (version.json, CHANGELOG.md). It PROPOSES.
#   2. release-manifest.json DECIDES. Required on release/*, optional on
#      hotfix/*. Its 'include' list replaces the build set outright, and anything
#      detection found changed must appear in 'include' or 'exclude' or the build
#      fails.
#
# Selecting on "version.json differs from main-v<N>" was tried first and does not
# work. /post-release-cleanup bumps each released package's patch on dev, so that
# test reported every previously-released package as shipping on every subsequent
# release branch, while a package whose version had never moved could not be
# selected at all. This arrangement mirrors Umbraco.AI.
# ============================================================================

param(
    [string]$SourceBranch = $env:BUILD_SOURCEBRANCH,
    [string]$RootPath = (Get-Location).Path,

    # Overrides the "last released state" ref that a release/hotfix build computes
    # its merge-base against. Defaults to origin/main-v<N>, derived from the branch.
    #
    # This is load-bearing, not a debug convenience: there are no automated tests
    # for the selection logic, so /release-management dry-runs this script with
    # this parameter before pushing a release branch. Keep it working.
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

# Files a release touches as bookkeeping rather than as a change. A package whose
# only diff since the released state is one of these has nothing new to publish.
#
# This is the whole fix for the carry-over problem: /post-release-cleanup bumps
# each released package's patch on dev so preview builds sort above the release,
# and that bump used to make the package look like it was shipping on every
# release branch cut afterwards, forever.
#
# Client/public/umbraco-package.json is deliberately NOT here. It also declares
# the backoffice extensions, so ignoring it wholesale would hide a real change
# such as adding an entry point. It is kept at a fixed placeholder version
# instead, so a release never touches it - see UpdatePackageManifestVersion in
# the root Directory.Build.targets, which stamps the real version into the
# wwwroot copy on CI builds.
$NonSubstantiveFiles = @("version.json", "CHANGELOG.md")

function Test-SubstantiveChange {
    <#
    .SYNOPSIS
    True when a changed file represents real content rather than release bookkeeping.
    #>
    param([string]$FilePath)

    return $NonSubstantiveFiles -notcontains (Split-Path $FilePath -Leaf)
}

function Get-ReleaseChangedProducts {
    <#
    .SYNOPSIS
    On a release/hotfix branch, reports which packages have substantive changes
    since the released state on main-v<N>.

    .DESCRIPTION
    This is a cross-check, not the decision. It exists so that a package which
    genuinely changed cannot be left out of release-manifest.json by accident.
    The manifest decides what actually packs - see Get-ManifestSelection.

    Comparison is against main-v<N> rather than each package's own release tag
    (which is what Umbraco.AI does) because this repo's tag slugs are not
    consistent: both release/crm-activecampaign-* and release/crm-active-campaign-*
    exist, as do both release/shopify-1.2.0 and release/commerce-shopify-*.
    main-v<N> is the last released state for the line and /post-release-cleanup
    always merges into it, so it carries the same information with no slug
    guessing.
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
        $line = if ($SourceBranch -match '^refs/heads/(v\d+)/') { $Matches[1] } else { 'v17' }
        $releasedRef = "origin/main-$line"
    }

    Write-Host "  Release branch build - detecting substantive changes vs $releasedRef" -ForegroundColor Cyan

    $base = git merge-base $releasedRef HEAD 2>&1
    if ($LASTEXITCODE -ne 0) {
        # Without a base there is nothing to compare, so report everything as
        # changed. On a release branch the manifest still constrains what packs,
        # so this degrades to "the cross-check is uninformative" rather than
        # "everything ships".
        Write-Host "  ! merge-base with $releasedRef failed - treating all packages as changed" -ForegroundColor Yellow
        $Products.Keys | ForEach-Object { $changed[$_] = $true }
        return $changed
    }

    $base = $base.Trim()
    Write-Host "  Comparing $base..HEAD" -ForegroundColor Gray

    foreach ($name in ($Products.Keys | Sort-Object)) {
        $files = git diff --name-only "$base..HEAD" -- $Products[$name].Path 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  ! $name - git diff failed, treating as changed" -ForegroundColor Yellow
            $changed[$name] = $true
            continue
        }

        $files = @($files | Where-Object { $_ -is [string] -and $_ })
        $substantive = @($files | Where-Object { Test-SubstantiveChange -FilePath $_ })

        if ($substantive.Count -gt 0) {
            $changed[$name] = $true
            Write-Host "  * $name changed ($($substantive.Count) file(s), e.g. $($substantive[0]))" -ForegroundColor Green
        }
        elseif ($files.Count -gt 0) {
            Write-Host "    $name release bookkeeping only ($($files.Count) file(s)) - not a change" -ForegroundColor Gray
        }
        else {
            Write-Host "    $name unchanged" -ForegroundColor Gray
        }
    }

    return $changed
}

function Resolve-ManifestPackageNames {
    <#
    .SYNOPSIS
    Maps names from release-manifest.json to discovered package keys.

    .DESCRIPTION
    Matching is case-insensitive and returns the canonical git-tracked key. That
    is deliberate: git tracks
    ...SEO.GoogleSearchConsole.URLInspectionTool while working copies have
    appeared on disk as ...UrlInspectionTool, and a human writing the manifest
    should not have to know which casing won.
    #>
    param(
        [object[]]$Names,
        [string]$ListName,
        [hashtable]$Products
    )

    $byLower = @{}
    $Products.Keys | ForEach-Object { $byLower[$_.ToLower()] = $_ }

    $keys = @()
    foreach ($item in $Names) {
        if ($item -isnot [string] -or [string]::IsNullOrWhiteSpace($item)) {
            throw "release-manifest.json '$ListName' must contain only non-empty strings"
        }

        $key = $byLower[$item.Trim().ToLower()]
        if (-not $key) {
            throw "release-manifest.json '$ListName' names a package that does not exist or is not packable: $item"
        }

        if ($keys -notcontains $key) { $keys += $key }
    }

    return $keys
}

function Get-ReleaseManifest {
    <#
    .SYNOPSIS
    Loads and validates release-manifest.json. Returns $null when absent.

    .DESCRIPTION
    Object form only: { "include": [...], "exclude": [...] }. Umbraco.AI also
    accepts a bare array for backwards compatibility; there is no such history
    here, so it is not supported.

    The manifest carries names only, never versions - version.json stays the
    single source of truth for those.
    #>
    param(
        [string]$RootPath,
        [hashtable]$Products
    )

    $manifestPath = Join-Path $RootPath "release-manifest.json"
    if (-not (Test-Path $manifestPath)) { return $null }

    $raw = Get-Content $manifestPath -Raw
    try {
        $manifest = $raw | ConvertFrom-Json
    }
    catch {
        throw "Failed to parse release-manifest.json: $($_.Exception.Message)"
    }

    if ($null -eq $manifest -or $manifest -isnot [psobject] -or
        $manifest.PSObject.Properties.Name -notcontains 'include') {
        throw "release-manifest.json must be an object with an 'include' property, e.g. { `"include`": [`"Umbraco.Cms.Integrations.Crm.Dynamics`"] }"
    }

    $includeKeys = Resolve-ManifestPackageNames -Names @($manifest.include) -ListName "include" -Products $Products

    $excludeKeys = @()
    if ($manifest.PSObject.Properties.Name -contains 'exclude' -and $null -ne $manifest.exclude) {
        $excludeKeys = Resolve-ManifestPackageNames -Names @($manifest.exclude) -ListName "exclude" -Products $Products
    }

    $overlap = @($includeKeys | Where-Object { $excludeKeys -contains $_ })
    if ($overlap.Count -gt 0) {
        throw "release-manifest.json lists the same package in both 'include' and 'exclude': $($overlap -join ', ')"
    }

    if ($includeKeys.Count -eq 0) {
        throw "release-manifest.json 'include' must name at least one package - a release branch exists to publish something"
    }

    return @{ Include = $includeKeys; Exclude = $excludeKeys }
}

function Get-ManifestSelection {
    <#
    .SYNOPSIS
    Applies release-manifest.json to the detected change set and returns the
    final selection.

    .DESCRIPTION
    Required on v<N>/release/*, optional on v<N>/hotfix/*, ignored everywhere
    else. When present it is the decision: 'include' replaces the build list
    outright, so a package with no changes at all still packs if listed. That is
    what lets a package which has never shipped be released without first
    inventing a version bump for it.

    Detection stays useful as a guard: anything it found changed must appear in
    'include' or 'exclude', or this throws. Forgetting a package is the expensive
    mistake, so it is the one made loud.
    #>
    param(
        [hashtable]$Products,
        [hashtable]$Changed,
        [string]$SourceBranch,
        [string]$RootPath
    )

    $isRelease = $SourceBranch -match '^refs/heads/v\d+/release/'
    $isHotfix = $SourceBranch -match '^refs/heads/v\d+/hotfix/'

    if (-not ($isRelease -or $isHotfix)) { return $Changed }

    $manifest = Get-ReleaseManifest -RootPath $RootPath -Products $Products

    if (-not $manifest) {
        if ($isRelease) {
            throw "release-manifest.json is required on a release branch. Run /release-management, or generate it with scripts/generate-release-manifest.ps1."
        }

        Write-Host ""
        Write-Host "  Hotfix branch with no release-manifest.json - using change detection" -ForegroundColor Gray
        return $Changed
    }

    Write-Host ""
    Write-Host "Applying release-manifest.json..." -ForegroundColor Cyan

    $changedKeys = @($Changed.Keys | Where-Object { $Changed[$_] })
    $unaccounted = @($changedKeys | Where-Object {
        $manifest.Include -notcontains $_ -and $manifest.Exclude -notcontains $_
    })

    if ($unaccounted.Count -gt 0) {
        throw "release-manifest.json does not account for changed package(s) - add each to 'include' or 'exclude': $(($unaccounted | Sort-Object) -join ', ')"
    }

    if ($manifest.Exclude.Count -gt 0) {
        Write-Host "  Explicitly excluded: $(($manifest.Exclude | Sort-Object) -join ', ')" -ForegroundColor DarkYellow
    }

    $selected = @{}
    $Products.Keys | ForEach-Object { $selected[$_] = $false }
    foreach ($key in $manifest.Include) { $selected[$key] = $true }

    Write-Host "  Releasing: $(($manifest.Include | Sort-Object) -join ', ')" -ForegroundColor Yellow

    # A package in 'include' that detection saw no change in is legitimate - it is
    # how a never-released package ships, and how a rebuild of an already
    # merged-back release branch still works - but say so, because the other
    # reason to see this is a mistake.
    foreach ($key in ($manifest.Include | Sort-Object)) {
        if (-not $Changed[$key]) {
            Write-Host "  note: $key has no substantive changes but is being released" -ForegroundColor DarkYellow
        }
    }

    return $selected
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
    $line = if ($SourceBranch -match '^refs/heads/(v\d+)/') { $Matches[1] } else { 'v17' }
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

    # A release/hotfix branch compares against the released state rather than
    # against a previous commit, and ignores release bookkeeping when doing so.
    if (Test-IsReleaseBranch -SourceBranch $SourceBranch) {
        return Get-ReleaseChangedProducts -Products $Products -SourceBranch $SourceBranch -CompareRef $ReleaseCompareRef
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

# Detection proposes; on a release/hotfix branch the manifest decides.
$changed = Get-ManifestSelection -Products $products -Changed $changed `
    -SourceBranch $SourceBranch -RootPath $RootPath

Write-PipelineVariables -Products $products -Changed $changed

Write-Host ""
Write-Host "Change detection complete" -ForegroundColor Green
