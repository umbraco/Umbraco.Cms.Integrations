# Contributing to Umbraco.Cms.Integrations

This repository holds the official Umbraco CMS integration packages. Each package
is independent: it has its own version, ships its own NuGet package, and is
released on its own schedule. There is no shared runtime library between them.

> **Which branch?** This guide describes the **v18 line** (`main-v18` / `v18/dev`).
> The v17 line is being brought onto the same setup; the legacy `main` branch
> (Umbraco 10-13) still uses the older manual process.

---

## Branches

| Branch | Purpose |
|---|---|
| `v18/dev` | Working branch. All changes land here first. |
| `main-v18` | Reflects the **last released** state. Never commit working changes here. |
| `v18/feature/*` | Feature branches, cut from and merged back to `v18/dev`. |

The flow is: work on a feature branch → PR into `v18/dev` → release from `v18/dev`
→ merge `v18/dev` into `main-v18` so it matches what shipped.

---

## Getting set up

```bash
git clone https://github.com/umbraco/Umbraco.Cms.Integrations.git
cd Umbraco.Cms.Integrations
git checkout v18/dev
dotnet build Umbraco.Cms.Integrations.slnx
```

Requires the **.NET 10 SDK** and **Node 22+** (for the packages that build client
assets).

To compile the .NET side only and skip the npm/Vite client build:

```bash
dotnet build Umbraco.Cms.Integrations.slnx -p:ShouldRunClientAssetsBuild=false
```

Use `examples/Umbraco.Cms.Integrations.Testsite.V18` to run the packages in a real
Umbraco instance.

---

## Dependency versions

Dependency versions are **centrally managed**. Do not put a `Version` attribute on
a `PackageReference` — it will fail the build with `NU1008`.

- Declare the version once in the root `Directory.Packages.props`.
- Reference it by name only in the `.csproj`:

```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="Algolia.Search" Version="6.13.0" />

<!-- the package's .csproj -->
<PackageReference Include="Algolia.Search" />
```

Bumping the Umbraco CMS version is therefore a one-line change that applies to
every package.

> `tests/` and `examples/Umbraco.Cms.Integrations.Testsite.V18/` each have their own
> scoped `Directory.Packages.props` and are deliberately outside the root one.

---

## Versioning

Each package folder has a `version.json` ([Nerdbank.GitVersioning][nbgv]) holding
the full `major.minor.patch`. **That file is the only place a version is written by
hand.**

```jsonc
// src/Umbraco.Cms.Integrations.Search.Algolia/version.json
{
    "version": "7.0.1",
    "publicReleaseRefSpec": ["^refs/heads/main-v18$"]
    // ...
}
```

Alongside it, `Directory.Build.props` pins `GitVersionBaseDirectory` to that
folder, so each package's version is computed from its own commits only — activity
in one package never moves another package's version.

### Which number to bump

Based on the commits affecting that package, using [Conventional Commits][cc]:

| Commits since the last release | Bump | Example |
|---|---|---|
| A breaking change (`feat!:`, or `BREAKING CHANGE:` in the body) | **Major** | `7.0.1` → `8.0.0` |
| `feat:` | **Minor** | `7.0.1` → `7.1.0` |
| `fix:` or `perf:` | **Patch** | `7.0.1` → `7.0.2` |
| Only `docs:` / `chore:` / `refactor:` | Usually none | — |

Each package keeps **one major per Umbraco major** (e.g. Search.Algolia `6.x` = v17,
`7.x` = v18). A new Umbraco major means a new major for every package.

### What you must NOT hand-edit

- **`<Version>` in the `.csproj`** — removed; the version comes from `version.json`.
- **`wwwroot/umbraco-package.json`** — CI stamps the computed version into it.
- **`Client/public/umbraco-package.json`** — keeps a placeholder version.

On `main-v18` a build produces a clean version (`7.0.2`). On any other branch NBGV
appends a preview suffix (`7.0.2--preview.4.gabc1234`), so dev builds always sort
below the release.

---

## Changelogs

Every package has a `CHANGELOG.md` in the [Keep a Changelog][kac] format. **If you
bump a `version.json`, add a matching entry** headed with the new version:

```markdown
## [7.1.0] - 2026-08-12

### feat

* **algolia:** Add support for filtered replica indices
```

CI fails the build if a `version.json` changed without a matching
`## [<version>]` entry. Only bumped packages are checked.

---

## CI

One pipeline, `azure-pipelines.yml`, covers the whole repo:

1. **Detect changed packages** — `.azure-pipelines/scripts/detect-changes.ps1`
   discovers packages (any `src/` folder with both a `.csproj` and a
   `version.json`) and diffs against the right base for the build context.
2. **Validate changelogs** — `.azure-pipelines/scripts/validate-changelogs.ps1`.
3. **Build & pack** — fans out over only the changed packages.

The pipeline builds and packs artifacts. It does **not** push to NuGet or create
tags; those are separate steps (see below).

### Adding a new package

No pipeline change is needed. Create `src/Umbraco.Cms.Integrations.<Area>.<Name>/`
containing:

- `<name>.csproj` (no `<Version>`, no `PackageReference` versions)
- `version.json` — this is what marks the folder as a package to build
- `Directory.Build.props` with `GitVersionBaseDirectory`
- `CHANGELOG.md`
- `readme.md`, `docs/readme.md`, `umbraco-marketplace-readme.md`

Then add it to `Umbraco.Cms.Integrations.slnx`.

### Gotchas

- **Committed `wwwroot` must match a fresh client build.** When you change
  `Client/src`, rebuild (`npm install && npm run build`) and commit the result, or
  the .NET `StaticWebAssets` step fails on a stale chunk hash.
- **cdxgen needs `--spec-version 1.6`** pinned, or the SBOM step fails.

---

## Releasing

Two Claude Code skills guide the process:

- **`/release-management`** — detects changed packages, recommends the bump per the
  table above, updates `version.json`, writes the changelog entry, and commits.
- **`/post-release-cleanup`** — merges `v18/dev` into `main-v18` and bumps the
  patch again on `v18/dev` so the next dev build sorts above the release.

Between the two, done manually:

1. Promote the CI-built artifact to NuGet.
2. Create an annotated tag: `release/<slug>-<version>`
   (e.g. `release/search-algolia-7.1.0`).
3. Create a GitHub release titled with the tag, linking the relevant PRs.

> Tagging is easy to forget and has been missed before. Do steps 2 and 3 every time.

---

## Reporting security issues

**Do not open a public GitHub issue for a security vulnerability.** Report it
privately via
[How to report a vulnerability in Umbraco](https://umbraco.com/trust-center/security-and-umbraco/how-to-report-a-vulnerability-in-umbraco/),
and do not disclose it publicly until a fix has shipped.

[nbgv]: https://github.com/dotnet/Nerdbank.GitVersioning
[cc]: https://www.conventionalcommits.org/
[kac]: https://keepachangelog.com/en/1.0.0/
