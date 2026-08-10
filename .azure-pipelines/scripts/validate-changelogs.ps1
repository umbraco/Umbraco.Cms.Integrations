# ============================================================================
# Umbraco.Cms.Integrations changelog validation
# ============================================================================
# Fails the build when a package's version.json version changed but its
# CHANGELOG.md has no matching entry - i.e. someone bumped a version without
# recording what changed.
#
# Only packages whose version.json actually changed in this diff are checked,
# so unrelated packages never block a build.
# ============================================================================

param(
    [string]$SourceBranch = $env:BUILD_SOURCEBRANCH,
    [string]$RootPath = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

function Get-ComparisonBase {
    param([string]$SourceBranch)

    if ($env:SYSTEM_PULLREQUEST_TARGETBRANCH) {
        $target = $env:SYSTEM_PULLREQUEST_TARGETBRANCH -replace '^refs/heads/', ''
        $mergeBase = git merge-base "origin/$target" HEAD 2>&1
        if ($LASTEXITCODE -eq 0) { return $mergeBase.Trim() }
        return $null
    }

    # Release/hotfix branch: compare against the released state, so every bump the
    # release carries is checked - not just the most recent commit's worth.
    if ($SourceBranch -match '^refs/heads/(v\d+)/(release|hotfix)/') {
        $mainBranch = "main-$($Matches[1])"
        $mergeBase = git merge-base "origin/$mainBranch" HEAD 2>&1
        if ($LASTEXITCODE -eq 0) { return $mergeBase.Trim() }
        return $null
    }

    if ($SourceBranch -match '^refs/heads/(main-v\d+|v\d+/dev)$') {
        return "HEAD~1"
    }

    $line = if ($SourceBranch -match '^refs/heads/(v\d+)/') { $Matches[1] } else { 'v17' }
    $mergeBase = git merge-base "origin/$line/dev" HEAD 2>&1
    if ($LASTEXITCODE -eq 0) { return $mergeBase.Trim() }
    return $null
}

function Get-VersionFromJson {
    param([string]$Path)

    if (-not (Test-Path $Path)) { return $null }
    try {
        return (Get-Content $Path -Raw | ConvertFrom-Json).version
    }
    catch {
        throw "Failed to parse ${Path}: $($_.Exception.Message)"
    }
}

Write-Host "======================================="
Write-Host "Umbraco.Cms.Integrations changelog validation"
Write-Host "======================================="
Write-Host "Source branch: $SourceBranch"
Write-Host ""

$base = Get-ComparisonBase -SourceBranch $SourceBranch
if (-not $base) {
    Write-Host "No usable comparison base - skipping changelog validation" -ForegroundColor Yellow
    exit 0
}

$changedFiles = git diff --name-only "$base" HEAD 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "git diff failed - skipping changelog validation" -ForegroundColor Yellow
    exit 0
}

$changedVersionFiles = @(
    $changedFiles |
        Where-Object { $_ -is [string] } |
        Where-Object { $_ -match '^src/[^/]+/version\.json$' }
)

if ($changedVersionFiles.Count -eq 0) {
    Write-Host "No version.json changes in $base..HEAD - nothing to validate" -ForegroundColor Green
    exit 0
}

Write-Host "Validating $($changedVersionFiles.Count) bumped package(s):" -ForegroundColor Cyan

$problems = @()

foreach ($versionFile in $changedVersionFiles) {
    $packageDir = Split-Path $versionFile -Parent
    $packageName = Split-Path $packageDir -Leaf

    # git diff reports deletions too. A removed version.json means the package is
    # leaving the line (or has not joined it yet) - there is no version to
    # document, so there is nothing to validate. Without this the file shows up as
    # "changed", the read returns nothing, and the build fails on a package that
    # is deliberately absent.
    $versionPath = Join-Path $RootPath $versionFile
    if (-not (Test-Path $versionPath)) {
        Write-Host "  skip $packageName (version.json removed)" -ForegroundColor DarkGray
        continue
    }

    $version = Get-VersionFromJson -Path $versionPath
    if (-not $version) {
        $problems += "$packageName : version.json has no 'version' value"
        continue
    }

    $changelogPath = Join-Path $RootPath (Join-Path $packageDir "CHANGELOG.md")
    if (-not (Test-Path $changelogPath)) {
        $problems += "$packageName : bumped to $version but has no CHANGELOG.md"
        continue
    }

    # Accept "## [1.2.3]" with anything after it (a date, a compare link, "Unreleased").
    $changelog = Get-Content $changelogPath -Raw
    $pattern = '(?m)^##\s*\[' + [regex]::Escape($version) + '\]'

    if ($changelog -match $pattern) {
        Write-Host "  OK   $packageName $version" -ForegroundColor Green
    }
    else {
        $problems += "$packageName : bumped to $version but CHANGELOG.md has no '## [$version]' entry"
        Write-Host "  FAIL $packageName $version" -ForegroundColor Red
    }
}

if ($problems.Count -gt 0) {
    Write-Host ""
    Write-Host "Changelog validation failed:" -ForegroundColor Red
    $problems | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    Write-Host ""
    Write-Host "Add an entry headed '## [<version>]' to the package's CHANGELOG.md." -ForegroundColor Yellow
    throw "Changelog validation failed for $($problems.Count) package(s)"
}

Write-Host ""
Write-Host "Changelog validation passed" -ForegroundColor Green
