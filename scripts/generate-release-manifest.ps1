# ============================================================================
# Umbraco.Cms.Integrations release manifest generator
# ============================================================================
# Writes release-manifest.json at the repo root, naming the packages a release
# branch will publish.
#
# The manifest is what CI treats as the decision on a release branch - see
# Get-ManifestSelection in .azure-pipelines/scripts/detect-changes.ps1. Change
# detection there is only a cross-check.
#
# This lives in scripts/ rather than .azure-pipelines/scripts/ on purpose: it is
# a release-manager tool, not a CI script, and detect-changes.ps1 treats any
# change under .azure-pipelines/ as affecting every package - so keeping it here
# means editing it does not trigger a full nine-package rebuild.
#
# Usage:
#   pwsh -File scripts/generate-release-manifest.ps1 -Include Umbraco.Cms.Integrations.Crm.Dynamics
#   pwsh -File scripts/generate-release-manifest.ps1 -Include A,B -Exclude C
#   pwsh -File scripts/generate-release-manifest.ps1            # numbered menu
# ============================================================================

param(
    # Packages to publish. Omit for an interactive numbered menu.
    [string[]]$Include = @(),

    # Packages that changed but are deliberately not being published. CI fails if
    # a changed package appears in neither list, so this is how you say "yes, I
    # know, not this one".
    [string[]]$Exclude = @(),

    [string]$RootPath = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

function Split-NameList {
    <#
    .SYNOPSIS
    Flattens a name list, splitting any comma-separated element.

    .DESCRIPTION
    `pwsh -File` mangles list arguments two different ways, and both are forms a
    human will reasonably type:

      -Include A,B,C      arrives as the single string "A,B,C" - the binder does
                          not split it for a [string[]] parameter under -File
      -Include "A","B"    the CALLING shell joins the array with spaces before
                          handing it to a native command, so it arrives as "A B"

    That second form also arrives with the quote characters still attached, so they
    are stripped too.

    Splitting on commas and whitespace, then stripping quotes, makes every form
    work. Safe because no package folder name contains any of those characters.
    #>
    param([string[]]$Values)

    $out = @()
    foreach ($value in $Values) {
        foreach ($part in ($value -split '[,\s]+')) {
            $trimmed = $part.Trim().Trim('"', "'")
            if ($trimmed) { $out += $trimmed }
        }
    }

    return $out
}

function Get-PackableProducts {
    <#
    .SYNOPSIS
    Discovers packable packages under src/, keyed by the casing git tracks.

    .DESCRIPTION
    Same rule as detect-changes.ps1: a folder qualifies when it contains both
    <Name>.csproj and version.json. The two must agree, or a manifest that
    validates here would fail in CI.

    Names are resolved to the casing git tracks, not the casing on disk. git is
    case-sensitive when looking paths up inside tree objects even on Windows, and
    this repo has ...SEO.GoogleSearchConsole.URLInspectionTool in the index while
    working copies have appeared as ...UrlInspectionTool.
    #>
    param([string]$RootPath)

    $srcPath = Join-Path $RootPath "src"
    if (-not (Test-Path $srcPath)) {
        throw "No src/ folder under $RootPath - run this from the repo root."
    }

    $gitCasing = @{}
    git -C $RootPath ls-tree -d --name-only HEAD src/ 2>$null |
        Where-Object { $_ -is [string] } |
        ForEach-Object {
            $gitName = Split-Path $_ -Leaf
            $gitCasing[$gitName.ToLower()] = $gitName
        }

    $products = @()

    Get-ChildItem -Path $srcPath -Directory |
        Where-Object { $_.Name -like "Umbraco.Cms.Integrations.*" } |
        ForEach-Object {
            $name = $_.Name
            if (-not (Test-Path (Join-Path $_.FullName "$name.csproj"))) { return }
            if (-not (Test-Path (Join-Path $_.FullName "version.json"))) { return }

            $gitName = $gitCasing[$name.ToLower()]
            if (-not $gitName) { $gitName = $name }

            $version = (Get-Content (Join-Path $_.FullName "version.json") -Raw |
                ConvertFrom-Json).version

            $products += [pscustomobject]@{ Name = $gitName; Version = $version }
        }

    if ($products.Count -eq 0) {
        throw "No packable packages found under $srcPath - check that version.json files are present."
    }

    return @($products | Sort-Object Name)
}

function Resolve-ProductNames {
    <#
    .SYNOPSIS
    Validates supplied names against the discovered packages and returns the
    canonical git-tracked names.

    .DESCRIPTION
    Case-insensitive on purpose - see the casing note in Get-PackableProducts. A
    name that does not resolve is a terminating error, so a typo fails here
    rather than in CI.
    #>
    param(
        [string[]]$Names,
        [string]$ListName,
        [object[]]$Products
    )

    $byLower = @{}
    $Products | ForEach-Object { $byLower[$_.Name.ToLower()] = $_.Name }

    $resolved = @()
    foreach ($item in $Names) {
        if ([string]::IsNullOrWhiteSpace($item)) {
            throw "'$ListName' contains an empty package name."
        }

        $key = $byLower[$item.Trim().ToLower()]
        if (-not $key) {
            $known = ($Products | ForEach-Object { $_.Name }) -join "`n  "
            throw "'$ListName' names a package that does not exist or is not packable: $item`n`nKnown packages:`n  $known"
        }

        if ($resolved -notcontains $key) { $resolved += $key }
    }

    return $resolved
}

function Read-ProductSelection {
    <#
    .SYNOPSIS
    Numbered menu fallback when -Include was not supplied.
    #>
    param([object[]]$Products)

    Write-Host ""
    Write-Host "Select the packages this release publishes:" -ForegroundColor Cyan
    Write-Host ""

    for ($i = 0; $i -lt $Products.Count; $i++) {
        Write-Host ("  {0,2}. {1}  ({2})" -f ($i + 1), $Products[$i].Name, $Products[$i].Version)
    }

    Write-Host ""
    $answer = Read-Host "Numbers, comma or space separated (e.g. 1,4)"

    $picked = @()
    foreach ($token in ($answer -split '[,\s]+' | Where-Object { $_ })) {
        $n = 0
        if (-not [int]::TryParse($token, [ref]$n) -or $n -lt 1 -or $n -gt $Products.Count) {
            throw "'$token' is not one of the listed numbers."
        }
        $name = $Products[$n - 1].Name
        if ($picked -notcontains $name) { $picked += $name }
    }

    if ($picked.Count -eq 0) {
        throw "Nothing selected - a release branch exists to publish something."
    }

    return $picked
}

# ============================================================================
# Main
# ============================================================================

Write-Host "======================================="
Write-Host "Umbraco.Cms.Integrations release manifest"
Write-Host "======================================="

$products = Get-PackableProducts -RootPath $RootPath

$Include = Split-NameList -Values $Include
$Exclude = Split-NameList -Values $Exclude

$includeNames = if ($Include.Count -gt 0) {
    Resolve-ProductNames -Names $Include -ListName "Include" -Products $products
}
else {
    Read-ProductSelection -Products $products
}

$excludeNames = @()
if ($Exclude.Count -gt 0) {
    $excludeNames = Resolve-ProductNames -Names $Exclude -ListName "Exclude" -Products $products
}

$overlap = @($includeNames | Where-Object { $excludeNames -contains $_ })
if ($overlap.Count -gt 0) {
    throw "The same package cannot be in both Include and Exclude: $($overlap -join ', ')"
}

# Written as an ordered hashtable so include always precedes exclude in the file.
$manifest = [ordered]@{
    include = @($includeNames | Sort-Object)
    exclude = @($excludeNames | Sort-Object)
}

$manifestPath = Join-Path $RootPath "release-manifest.json"
$json = ($manifest | ConvertTo-Json -Depth 3) + "`n"
Set-Content -Path $manifestPath -Value $json -NoNewline -Encoding utf8

Write-Host ""
Write-Host "Wrote $manifestPath" -ForegroundColor Green
Write-Host ""
Write-Host "Publishing:" -ForegroundColor Cyan
foreach ($name in $manifest.include) {
    $version = ($products | Where-Object { $_.Name -eq $name }).Version
    Write-Host "  $name  $version" -ForegroundColor Green
}

if ($manifest.exclude.Count -gt 0) {
    Write-Host ""
    Write-Host "Deliberately not publishing:" -ForegroundColor DarkYellow
    foreach ($name in $manifest.exclude) {
        Write-Host "  $name" -ForegroundColor DarkYellow
    }
}

Write-Host ""
Write-Host "Commit this file on the release branch. /post-release-cleanup deletes it" -ForegroundColor Gray
Write-Host "before merging back, so it never reaches main-v<N> or v<N>/dev." -ForegroundColor Gray
