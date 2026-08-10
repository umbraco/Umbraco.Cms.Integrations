# CLAUDE.md

Repo-level guide for **Umbraco.Cms.Integrations** — a multi-package repository of official Umbraco CMS integration packages ("best of breed" / DXP initiative). Each package ships to NuGet and installs as an Umbraco backoffice extension.

This file is for **navigation, structure, and release workflow**. Package-specific detail lives in each package's `readme.md` / `docs/`.

---

## 1. Overview

- **What it is:** 9 independent integration packages (CRM, commerce, search, SEO, automation, analytics), each versioned and released separately to NuGet.
- **Stack:** .NET (net10.0 on the v17/v18 lines), Umbraco CMS as the host, per-package **TypeScript + Vite** backoffice client compiled into `wwwroot`, built/packed via **Azure Pipelines**.
- **Not a single product:** there is no shared runtime library across packages — they are sibling projects that share only the repo, tooling, and release conventions. No package references another.

> **v18 vs v17.** The v18 line has moved to central package management, per-package `version.json` (NBGV) and a single pipeline — described below. **The v17 line and legacy `main` still use the older manual process** (hand-edited `<Version>` in three files, one pipeline per package). Sections 3 and 4 flag where the two differ.

---

## 2. Repository Structure

```
/src              - The integration packages (one .NET project per package)
/tests            - Test projects, `<PackageName>.Tests` (sparse; only Crm.Hubspot so far)
/examples         - Test sites (e.g. Umbraco.Cms.Integrations.Testsite.V18)
/.azure-pipelines - Pipeline scripts + templates (v18 line)
azure-pipelines.yml       - ONE pipeline for the whole repo (v18 line)
Directory.Packages.props  - Central dependency versions (v18 line)
Umbraco.Cms.Integrations.slnx
package.json      - Root npm workspace over every package's Client/ (v18 line)
package-lock.json - The ONE lockfile for all clients
CONTRIBUTING.md   - Contributor-facing version of much of this file
```

Each package under `src/` follows the same shape:
```
src/Umbraco.Cms.Integrations.<Area>.<Name>/
  <project>.csproj                     - .NET class library; NO <Version>, NO PackageReference versions
  version.json                         - NBGV config; the ONLY hand-edited version (v18 line)
  Directory.Build.props                - pins GitVersionBaseDirectory to this folder
  CHANGELOG.md                         - Keep a Changelog format; required when bumping
  Client/                              - TypeScript + Vite source; an npm workspace of the
                                         root package.json (no per-package lockfile)
    src/, public/umbraco-package.json  - backoffice manifest; SOURCE copy, carries a real
                                         version, NOT stamped by CI - keep in step by hand
  wwwroot/                             - COMPILED client assets - GITIGNORED, never committed
                                         (+ umbraco-package.json, copied by Vite, CI-stamped)
  readme.md, docs/, umbraco-marketplace-readme.md
```

> A `src/` folder counts as a package to CI **only if it has both a `.csproj` and a `version.json`**. That is what keeps stale folders out of the build, and it means adding a package needs no pipeline change.

### Client assets (v18 line)

**`wwwroot` is build output and is not in git.** Every package's `Client/` is a workspace of the root `package.json`, so there is one `package-lock.json` for the whole repo:

```bash
npm ci                              # installs every client, once
npm run build                       # builds every client into its own wwwroot
npm run build --workspace algolia   # or just one (workspace names: algolia, hubspot,
                                    # shopify, dynamics, semrush, zapier,
                                    # activecampaign, googlesearchconsole)
```

`dotnet build` does **not** build clients — the previous `Microsoft.AspNetCore.ClientAssets.targets` import and its `ShouldRunClientAssetsBuild` switch are gone from every csproj, along with the generated `Client/config.outputPath.js` (Vite now writes to a literal `../wwwroot`). This matches `Umbraco.Automate` and `Umbraco.AI`, which both gitignore `wwwroot` and drive npm from the pipeline.

Consequences worth knowing:

- **A fresh clone has no `wwwroot`.** Run `npm ci && npm run build` before running the test site, or the backoffice loads no extensions.
- **Packing without building fails loudly.** An `EnsureClientAssetsBuilt` target in the root `Directory.Build.targets` errors if a package has a `Client/` but no built `wwwroot`, so a package can never ship with its assets missing.
- **CI builds only the workspace it needs.** `detect-changes.ps1` reads each `Client/package.json` name into the matrix as `clientName`, and the pack template installs and builds just that workspace (`npm ci --workspace <clientName> --include-workspace-root`). npm's download cache is restored per client via `Cache@2`, keyed on `package-lock.json` — per client rather than once for the repo, because Azure cache entries are immutable and a shared key would only ever serve whichever job finished first.
- Adding a package with a client means adding its `Client` folder to the root `package.json` `workspaces` array — use the casing **git** tracks (`...GoogleSearchConsole.URLInspectionTool`), not the disk casing.

### Packages (v17/v18 lines)

| Project | Area | v17 major | v18 major |
|---|---|---|---|
| `Crm.Hubspot` | CRM | 8.x | 9.x |
| `Crm.ActiveCampaign` | CRM | 6.x | 7.x |
| `Crm.Dynamics` | CRM | 5.x | 6.x |
| `Commerce.Shopify` | Commerce | 5.x | 6.x |
| `Search.Algolia` | Search | 6.x | 7.x |
| `SEO.Semrush` | SEO | 4.x | 5.x |
| `SEO.GoogleSearchConsole.URLInspectionTool` | SEO | — | 3.x |
| `Automation.Zapier` | Automation | 5.x | 6.x |
| `Analytics.Cookiebot` | Analytics | 3.x (planned) | 4.x |

**Cookiebot** was dormant: it was last maintained on `main-v15` at 2.0.1 (Umbraco 15–16) and was absent from the v17/v18 lines, which carried only a dead `azure-pipeline - Script.Cookiebot.yml` pointing at a project that did not exist. It has been brought onto v18 at `4.0.0`. It is a plain Razor Class Library — server-side banner/declaration partial views, no TypeScript client and no `wwwroot`.

> The legacy `main` branch (Umbraco 10–13) additionally carries packages on no active line (PIM.Inriver, DAM.Aprimo, Crm.ActiveCampaign.Core, Commerce.CommerceTools). Per [UmbracoDocs#8300](https://github.com/umbraco/UmbracoDocs/pull/8300) these support Umbraco 13 or lower and their docs are being removed; Cookiebot was deliberately kept.

---

## 3. Branching & Release Model

**One branch pair per Umbraco major.** For each supported Umbraco version there is a `main-v<N>` and a `v<N>/dev`:

- **`main-v<N>`** — reflects the **last released** state for that major.
- **`v<N>/dev`** — the **working branch**; always ahead of `main-v<N>` with unreleased work.
- **`v<N>/release/YYYY.MM.N`** — cut from dev per release; **this is what builds publishable artifacts** (v18 line).
- **`v<N>/hotfix/YYYY.MM.N`** — cut from `main-v<N>` for an urgent fix on the released state (v18 line).
- **`v<N>/feature/*`** — feature branches, cut from and merged back to `v<N>/dev`.
- **`main`** (no suffix) — the **legacy Umbraco 10–13** line.

**Flow:** work lands on `v<N>/dev` → a **release branch** is cut from dev → the pipeline builds *that branch* to get clean versions → published to NuGet and tagged → the release branch is merged back into **both** `main-v<N>` (so it matches what shipped) and `v<N>/dev`. Do **not** commit release/working changes directly to `main-v<N>` — it re-diverges it from dev.

### Versioning — v18 line (NBGV)

Each package has its own `version.json` ([Nerdbank.GitVersioning][nbgv]) holding the full `major.minor.patch`. **That is the only place a version is written by hand.** A sibling `Directory.Build.props` pins `GitVersionBaseDirectory` to the package folder, so each package's version is computed from its own commits — activity in one package never moves another's.

Bump rule, from the commits affecting that package ([Conventional Commits][cc]):

| Commit signal | Bump |
|---|---|
| `BREAKING CHANGE:` in body, or `!` after type/scope | Major |
| `feat:` | Minor |
| `fix:` / `perf:` | Patch |
| Only `docs:` / `chore:` / `refactor:` | Usually none |

**Do not hand-edit:** the csproj (`<Version>` is gone) or `wwwroot/umbraco-package.json` — an `UpdatePackageManifestVersion` target in the root `Directory.Build.targets` stamps the latter on CI builds, so the committed value is always overwritten in the published package.

**Do keep in step:** `Client/public/umbraco-package.json`. It is the source manifest the Vite build copies into `wwwroot`, and CI does **not** stamp it. Every package carries a real version there, so bump it alongside `version.json`. Its value never reaches the published package (the `wwwroot` copy is stamped after the copy — verified with the client build enabled), but drift is confusing: `Search.Algolia` sat at `7.0.0` while shipping as `7.0.1`.

> Reducing this to a true placeholder (as `Umbraco.AI` does) would remove the last hand-maintained version file. Not done here — it would touch all 9 packages and was out of scope.

On `main-v18` a build produces a clean version (`7.0.2`). On any other branch NBGV appends a preview suffix (`7.0.2--preview.4.gabc1234`), which sorts **below** the release — hence the post-release patch bump on dev.

Each package keeps **one major per Umbraco major** (see the table in section 2).

### Versioning — v17 line and legacy `main` (manual)

Still the old way: pipelines pack with `versioningScheme: off` and the version comes from the csproj `<Version>`. Bumping means editing **all three**:

1. `<project>.csproj` → `<Version>`
2. `Client/public/umbraco-package.json` → `"version"` (source)
3. `wwwroot/umbraco-package.json` → `"version"` (built copy)

### Dependency versions — v18 line

Managed centrally in the root `Directory.Packages.props`. A `PackageReference` **must not** carry a `Version` attribute (`NU1008` otherwise) — declare it once centrally and reference by name. Bumping Umbraco CMS is therefore one line.

`tests/` and `examples/Umbraco.Cms.Integrations.Testsite.V18/` each have their own scoped `Directory.Packages.props` and sit outside the root one.

### Releasing (per package)

The pipeline **only builds + packs the `.nupkg` + SBOM as artifacts** — it does **not** push to NuGet or create tags.

> **A release artifact is built from a release branch.** `publicReleaseRefSpec` lists `main-v18`, `v18/release/*` and `v18/hotfix/*`, so a build on any of those drops the preview suffix (`7.1.0`); a `v18/dev` or feature-branch build gives `7.1.0--preview.N.gSHA`. Publishing a dev artifact would put a preview version on NuGet. This mirrors `Umbraco.Automate`/`Umbraco.AI`, which use the same three-pattern spec (with `v18/main` in place of `main-v18`).
>
> This is a real behavioural difference from the old manual process, where the csproj `<Version>` was clean on every branch and any build was publishable.

**Release branches:** `v<N>/release/YYYY.MM.N` (calendar-based, cut from `v<N>/dev`), and `v<N>/hotfix/YYYY.MM.N` for an urgent fix cut from `main-v<N>`. The name is independent of package versions — one branch can carry several packages at different versions. Note the branch (`v18/release/2026.08.1`) and the tags (`release/<slug>-<version>`) are different ref namespaces and do not collide.

**No release manifest.** Unlike `Umbraco.AI`, there is no `release-manifest.json`. On a release branch, CI treats a package as shipping when its `version.json` differs from `main-v<N>` — the bump itself is the declaration.

> **`main-v<N>` must carry `version.json` before the first release branch is cut.** The comparison is `git show main-v<N>:src/<pkg>/version.json`; when that file does not exist there, `detect-changes.ps1` treats the package as new to the line and force-includes it. Until this work is merged, `main-v18` has no `version.json` for any package, so a release branch cut now selects **all nine** regardless of what was bumped — and because a release branch drops the preview suffix, it would hand you nine clean-versioned `.nupkg` files at versions several of which are already on NuGet (`Search.Algolia` `7.0.1`, for one). Merge to `main-v<N>` first and this never arises; it is a one-time condition, not an ongoing hazard.

Two skills cover the repeatable parts:

- **`/release-management`** — detect changed packages, recommend the bump, **cut the release branch**, update `version.json`, write the `CHANGELOG.md` entry, push.
- **`/post-release-cleanup`** — merge the release branch into `main-v<N>` **and** back into `v<N>/dev`, bump each released package's patch on dev, optionally delete the branch.
- **`/changelog-management`** — changelog work on its own: preview what a package would release, backfill a missing entry, or fix a `validate-changelogs.ps1` failure. Unlike `Umbraco.Automate`/`Umbraco.AI` there is no generation script behind it; entries come from reading `git log`.

Full sequence:

1. `/release-management` on `v<N>/dev` — cuts `v<N>/release/YYYY.MM.N`, bumps, changelogs, pushes.
2. Pipeline builds the release branch. **Verify the artifact is clean-versioned** before continuing.
3. **Promote** it to NuGet (separate step).
4. Create an annotated tag `release/<slug>-<version>` (e.g. `release/crm-hubspot-9.0.1`).
5. Create a GitHub release (title = tag name), linking the relevant PRs.
6. `/post-release-cleanup` — merges back into both branches and bumps dev.

> Tagging/releasing is easy to forget — the v18 `.0.0` tags were missing entirely and had to be backfilled. Always do steps 3–5 after promoting.
>
> Tag slugs are **not** perfectly consistent in the existing history (both `crm-activecampaign` and `crm-active-campaign` exist; Shopify has both `shopify-1.2.0` and `commerce-shopify-*`). Confirm a real tag with `git tag --list` rather than deriving the slug.

---

## 4. CI (Azure Pipelines)

### v18 line — one pipeline

`azure-pipelines.yml` covers the whole repo in two stages:

1. **DetectChanges** — `.azure-pipelines/scripts/detect-changes.ps1` discovers packages (a `src/` folder with both a `.csproj` and a `version.json`), then selects what to build based on the branch:
   - **`v18/release/*` / `v18/hotfix/*`** — selects packages whose `version.json` differs from `main-v18`, i.e. those with a new version to publish. Throws if nothing was bumped.
   - **PRs** — diffs against the merge-base with the target branch.
   - **`main-v18` / `v18/dev`** — diffs against `HEAD~1`.
   - **feature branches** — diffs against the merge-base with `v18/dev`.

   A `Directory.Packages.props` change is traced to only the packages referencing the packages whose versions moved. Then `validate-changelogs.ps1` runs (on a release branch it compares against `main-v18`, so every bump the release carries is checked).

   > Package paths are resolved to the casing **git** tracks, not the casing on disk. `git show <ref>:<path>` is case-sensitive even on Windows, and this repo has `...GoogleSearchConsole.URLInspectionTool` in the index while working copies have appeared with `...UrlInspectionTool` on disk. Without this, that package was reported as brand new and force-included in every release.

2. **Test** — `dotnet test` over `tests/**/*Tests.csproj`, publishing `.trx` results and Cobertura coverage. **`Pack` depends on this stage**, so a failing test blocks packing and therefore blocks any promotion to NuGet.

   > Both the `Test` and `Pack` stage conditions are wrapped in `and(succeeded(), ...)`. A custom `condition:` **replaces** the implicit `succeeded()` check, so without it `Pack` would run even after `Test` failed.
3. **Pack** — a matrix over only the changed packages, one job each, via `.azure-pipelines/templates/pack-product.yml`. `nbgv cloud` runs with `workingDirectory` set to the package folder so it picks up that package's `version.json`.

Adding a package needs **no pipeline change** — give it a `version.json`.

### v17 line and legacy `main`

Still **one pipeline per package**, path-filtered to `src/<project>/**`.

### Gotchas

- **Changelog required on a bump.** CI fails when a `version.json` changed with no matching `## [<version>]` entry in that package's `CHANGELOG.md`. Only bumped packages are checked. (v18 line.)
- **SBOM (cdxgen):** the step fails unless `--spec-version 1.6` is pinned. Present on the active lines; the legacy `main` line does not have it.
- **`wwwroot` is gitignored** on the v18 line — see [Client assets](#client-assets-v18-line). It used to be committed, which meant every dependency resolution churned the Vite chunk hashes and a stale copy failed the .NET `StaticWebAssets` step. Neither problem exists now, and `-p:ShouldRunClientAssetsBuild=false` no longer does anything (`dotnet build` never touches npm).
- **A pack needs the client built first.** `npm ci && npm run build`, or the `EnsureClientAssetsBuilt` target fails the pack.
- **`NU1507`** is expected on the v18 line: central package management plus the two feeds in `NuGet.config`. A warning, not an error; silencing it needs package source mapping.
- **`dotnet test` arguments.** Do not pass `--logger`/`--results-directory` alongside `publishTestResults: true` — the `DotNetCoreCLI@2` task appends its own pair and `dotnet test` rejects two values for `--results-directory`.
- **NBGV needs full history.** Every job that builds a package must `checkout` with `fetchDepth: 0`; the agent default is a depth-1 fetch and NBGV throws "Shallow clone lacks the objects required to calculate version height".

---

## Quick Reference

**Key directories:** `/src` (packages) · `/tests` · `/examples` (test sites) · `/.azure-pipelines` (scripts + templates, v18)

**Branches:** `main-v18` / `v18/dev` (Umbraco 18) · `main-v17` / `v17/dev` (Umbraco 17) · `main` (legacy v10–13)

**Publishable builds come from** `main-v<N>`, `v<N>/release/*` or `v<N>/hotfix/*` only — everything else is `--preview` suffixed

**Release tag convention:** `release/<slug>-<version>` (annotated), e.g. `release/search-algolia-7.0.1`

**Bump a version:**
- **v18 line** → edit `src/<package>/version.json` only, and add a `CHANGELOG.md` entry
- **v17 / legacy** → edit 3 files: csproj `<Version>`, `Client/public/umbraco-package.json`, `wwwroot/umbraco-package.json`

**First run after a clone (v18 line) — `wwwroot` is not in git:**
```bash
npm ci
npm run build
```

**Build the .NET side** (never builds clients, so it is always the fast path):
```bash
dotnet build Umbraco.Cms.Integrations.slnx
```

**Run the CI checks locally (v18):**
```bash
pwsh -File .azure-pipelines/scripts/detect-changes.ps1
pwsh -File .azure-pipelines/scripts/validate-changelogs.ps1
```

[nbgv]: https://github.com/dotnet/Nerdbank.GitVersioning
[cc]: https://www.conventionalcommits.org/
