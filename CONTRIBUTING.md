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
| `v18/release/*` | Cut from `v18/dev` per release; builds the publishable artifacts. |
| `v18/hotfix/*` | Cut from `main-v18` for an urgent fix on the released state. |

The flow is: work on a feature branch → PR into `v18/dev` → cut a release branch
from `v18/dev` → publish from there → merge the release branch back into both
`main-v18` and `v18/dev`.

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

## Tests

```bash
dotnet test Umbraco.Cms.Integrations.slnx -p:ShouldRunClientAssetsBuild=false
```

Test projects live under `tests/`, one per package that has them, named
`<PackageName>.Tests`. Coverage is currently partial — only `Crm.Hubspot` has tests.

They use **xunit + Moq**, matching `Umbraco.Automate` and `Umbraco.AI` (the versions
are pinned in the root `Directory.Packages.props` to the same values those repos
use). Note this repo previously used NUnit; the HubSpot tests were rewritten when
the controllers they covered were restructured.

Writing tests for the management API controllers:

- Controllers take `IHttpClientFactory`, so fake HTTP responses by mocking
  `HttpMessageHandler` and handing back an `HttpClient` — see
  `TestHelpers.HttpClientFactory`.
- Controllers returning `IActionResult` need the result unwrapped
  (`Assert.IsType<OkObjectResult>(result)`); those returning a DTO can be asserted
  directly.
- `ILogger.LogInformation`/`LogError` are extension methods and cannot be verified
  directly. Use `TestHelpers.VerifyLogged`, which targets the underlying `Log` call.
- Shared fixtures and settings builders live in `TestHelpers`. `HubspotSettings`
  has `UseUmbracoAuthorization = true` by default, which matters when testing
  configuration validity — use `NoAuthorizationSettings()` for the "nothing
  configured" case.

CI runs the tests in their own stage, and **`Pack` depends on it** — a failing test
blocks packing, so nothing can be promoted to NuGet from a red build. Any project
added under `tests/` matching `*Tests.csproj` is picked up with no pipeline change.

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

**A release is built from a release branch.** `version.json` lists `main-v18`,
`v18/release/*` and `v18/hotfix/*` as public release refs, and NBGV only drops the
preview suffix on a branch matching one of them:

| Branch built | Version produced |
|---|---|
| `v18/release/2026.08.1` | `7.1.0` ← publishable |
| `main-v18` | `7.1.0` |
| `v18/dev`, feature branches | `7.1.0--preview.4.gabc1234` |

So the release branch is what produces the artifact you publish, and `main-v18` /
`v18/dev` stay untouched until the release is proven. This matches how
`Umbraco.Automate` and `Umbraco.AI` work.

Release branches are named `v<N>/release/YYYY.MM.N` (calendar-based, e.g.
`v18/release/2026.08.1`). That is independent of the package versions — one branch
can carry several packages at different versions. Urgent fixes on top of an
already-released state use `v<N>/hotfix/YYYY.MM.N`, cut from `main-v<N>`.

> **No release manifest.** On a release branch, CI treats a package as shipping when
> its `version.json` differs from `main-v18` — the bump *is* the declaration. There
> is no `include`/`exclude` list to maintain.

Two Claude Code skills guide the process:

- **`/release-management`** — detects changed packages, recommends the bump per the
  table above, cuts the release branch, updates `version.json`, writes the changelog
  entry, and pushes.
- **`/post-release-cleanup`** — merges the release branch into `main-v18` and back
  into `v18/dev`, bumps the patch on dev so the next dev build sorts above the
  release, and offers to delete the branch.

Full sequence:

1. `/release-management` on `v18/dev` — cuts `v18/release/YYYY.MM.N`, bumps,
   writes changelogs, pushes the branch.
2. CI builds the release branch. **Check the artifact is clean-versioned**
   (`7.1.0`, not `7.1.0--preview.N`) before continuing.
3. Promote the artifact to NuGet.
4. Create an annotated tag: `release/<slug>-<version>`
   (e.g. `release/search-algolia-7.1.0`).
5. Create a GitHub release titled with the tag, linking the relevant PRs.
6. `/post-release-cleanup` — merges back and bumps the dev versions.

> Tagging is easy to forget and has been missed before. Do steps 4 and 5 every time.
>
> The branch is `v18/release/<date>` while tags are `release/<slug>-<version>`. They
> live in different ref namespaces and do not collide.

---

## Reporting security issues

**Do not open a public GitHub issue for a security vulnerability.** Report it
privately via
[How to report a vulnerability in Umbraco](https://umbraco.com/trust-center/security-and-umbraco/how-to-report-a-vulnerability-in-umbraco/),
and do not disclose it publicly until a fix has shipped.

[nbgv]: https://github.com/dotnet/Nerdbank.GitVersioning
[cc]: https://www.conventionalcommits.org/
[kac]: https://keepachangelog.com/en/1.0.0/
