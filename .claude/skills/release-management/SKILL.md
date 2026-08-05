---
name: release-management
description: Prepares a release for one or more packages in this repo - detects which packages changed since their last release tag, recommends a version bump from the commit history, updates version.json, writes the changelog entry, and commits. Use when preparing a release.
---

# Release Manager

You prepare releases for **Umbraco.Cms.Integrations**.

## What makes this repo different

Adapted from the `Umbraco.AI` skill of the same name, with its inter-product
machinery removed, because this repo is shaped differently:

- **Packages are independent siblings.** None references another. There is no
  dependency cascade, no forced bumps, no `Directory.Packages.props` range
  updates and no npm peer-dependency syncing. If you find yourself reasoning
  about "which other packages does this one affect", stop - the answer is none.
- **No release branch.** Bumps are committed straight to `v18/dev` (or
  `v17/dev`). Do not create a `release/*` branch.
- **No release manifest.** `release-manifest.json` does not exist here; CI
  decides what to build from the file diff alone.
- **Tags are `release/<slug>-<version>`**, e.g. `release/search-algolia-7.1.0` -
  not `Product@Version`.

## Task

1. Detect which packages changed since their last release tag.
2. Recommend a version bump per package from the commit history.
3. Confirm with the user.
4. Update each package's `version.json`.
5. Write each package's `CHANGELOG.md` entry.
6. Review the changelogs for noise and completeness.
7. Validate and commit.

---

## Phase 1: Detect changed packages

Confirm the current branch is a dev branch:

```bash
git branch --show-current    # expect v18/dev or v17/dev
```

If it is `main-v18`/`main-v17`, stop and tell the user - releases are prepared on
the dev branch.

Discover the packages (any `src/` folder with both a `.csproj` and a
`version.json`):

```bash
ls -d src/*/ | while read d; do
  [ -f "$d/version.json" ] && basename "$d"
done
```

For each package, find its last release tag and the commits since. The tag slug
is the package name minus the `Umbraco.Cms.Integrations.` prefix, lowercased,
with dots replaced by hyphens (`Umbraco.Cms.Integrations.Search.Algolia` →
`search-algolia`).

> Slugs are not perfectly consistent in the existing tag history - both
> `crm-activecampaign` and `crm-active-campaign` exist. Always confirm the real
> tag with `git tag --list` rather than assuming the derived slug.

```bash
git tag --list "release/<slug>-*" --sort=-version:refname | head -n1
git log <tag>..HEAD --oneline -- src/<PackageName>/
```

Ignore changes that are only to `CHANGELOG.md` or `version.json` - those are
release-prep artifacts, not substantive changes.

Present what you found:

```
Changes since last release:

| Package                  | Last tag | Commits since                |
|--------------------------|----------|------------------------------|
| Search.Algolia           | 7.0.1    | 4 commits (2 fix, 1 feat)    |
| Crm.Hubspot              | 9.0.1    | 1 commit  (1 fix)            |
```

If a package has no release tag at all, say so explicitly and treat its
`version.json` value as the current version.

---

## Phase 2: Recommend a version bump

For each changed package, read its commits and apply this rule (highest wins):

| Commit signal | Bump |
|---|---|
| `BREAKING CHANGE:` in the body, or `!` after the type/scope | **Major** |
| `feat:` | **Minor** |
| `fix:` or `perf:` | **Patch** |
| Only `docs:` / `chore:` / `refactor:` / `test:` / `ci:` | **Ask the user** (default: no bump) |

Read the current version from the package's `version.json`, then:

- Major: increment X, reset Y and Z to 0
- Minor: increment Y, reset Z to 0
- Patch: increment Z

**Never** change a package's major to a number that does not match its Umbraco
line. Each package keeps one major per Umbraco major (Search.Algolia `6.x` = v17,
`7.x` = v18). If the commits genuinely warrant a major bump within a line, raise
it with the user rather than silently jumping the major - it would collide with
the next Umbraco major's numbering.

Present the recommendation:

```
| Package        | Current | Proposed | Reason              |
|----------------|---------|----------|---------------------|
| Search.Algolia | 7.0.1   | 7.1.0    | 1 feat, 2 fix       |
| Crm.Hubspot    | 9.0.1   | 9.0.2    | 1 fix               |
```

---

## Phase 3: Confirm with the user

Use **AskUserQuestion** to confirm. Options:

- **Use the recommended versions** (default)
- **Adjust individual versions** - ask per package, validate the `X.Y.Z` format,
  warn if it breaks the one-major-per-Umbraco-major convention
- **Drop a package from this release**
- **Cancel**

Do not proceed until the user has confirmed.

---

## Phase 4: Update version.json

For each confirmed package, edit **only** the `version` field of
`src/<PackageName>/version.json`. Leave every other property untouched.

Do **not** touch:

- the `.csproj` (it has no `<Version>` - the version comes from `version.json`)
- `wwwroot/umbraco-package.json` (CI stamps it)
- `Client/public/umbraco-package.json` (keeps a placeholder)

---

## Phase 5: Write the changelog entry

For each package, add a new entry at the top of `src/<PackageName>/CHANGELOG.md`,
below the header block, in [Keep a Changelog][kac] format:

```markdown
## [7.1.0] - 2026-08-12

### feat

* **algolia:** Add support for filtered replica indices ([abc1234](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/abc1234))

### fix

* **algolia:** Refresh the content cache for changed nodes ([def5678](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/def5678))
```

Rules:

- Use today's real date. Get it with `date +%Y-%m-%d` - do not guess.
- Group by commit type, using the type as the heading (`feat`, `fix`, `perf`).
- One bullet per user-facing change, taken from the commit subject.
- A breaking change gets its own `### Breaking` section, listed first.

---

## Phase 6: Review the changelogs

Before committing, review each entry you just wrote.

**Strip noise:**

- `refactor:`, `chore:`, `docs:`, `test:`, `ci:`, `build:` entries - these are not
  user-facing and must not appear in a public changelog. (An `### Internal` note
  is acceptable when a release would otherwise have an empty entry.)
- `Co-Authored-By:` lines and other commit-body metadata that leaked in.
- PR merge-commit subjects (`Merge pull request #123 from ...`).
- Test-only or build-only fixes.

**Check completeness** - compare the entry against what actually changed:

```bash
git diff <tag>..HEAD --name-only -- src/<PackageName>/ | grep -vE 'CHANGELOG.md|version.json'
```

If a file changed but nothing in the changelog explains it, either add an entry or
satisfy yourself it is genuinely internal. Commits scoped to another package can
still touch this one - those are the ones most often missed.

**Empty entries:** if a package's only changes are build/tooling, recommend
dropping it from the release and reverting its `version.json`.

Present the review, then apply fixes:

```
Changelog review:

Needs attention
- Search.Algolia: "Fix failing tests" is internal-only, removing
- Crm.Hubspot: no entry for the form-picker change (commit was scoped to dynamics)

Clean
- SEO.Semrush
```

---

## Phase 7: Validate and commit

Run the same check CI runs, so a failure surfaces here and not in the pipeline:

```bash
pwsh -File .azure-pipelines/scripts/validate-changelogs.ps1
```

Then confirm each bumped `version.json` matches its changelog heading, and commit:

```bash
git add src/*/version.json src/*/CHANGELOG.md
git commit -m "chore(release): Prepare release of <packages>

- Umbraco.Cms.Integrations.Search.Algolia: 7.0.1 -> 7.1.0
- Umbraco.Cms.Integrations.Crm.Hubspot: 9.0.1 -> 9.0.2"
```

Then report what remains manual:

```
Prepared and committed on v18/dev:
  - Search.Algolia 7.0.1 -> 7.1.0
  - Crm.Hubspot    9.0.1 -> 9.0.2

Still to do by hand:
  1. Push, and let CI build and pack the artifacts.
  2. Promote each artifact to NuGet.
  3. Tag each released package:
       git tag -a release/search-algolia-7.1.0 -m "Search.Algolia 7.1.0"
       git tag -a release/crm-hubspot-9.0.2   -m "Crm.Hubspot 9.0.2"
       git push origin --tags
  4. Create a GitHub release per tag, titled with the tag, linking the PRs.
  5. Run /post-release-cleanup to merge into main-v18 and bump dev versions.
```

Do not push, tag, or publish yourself unless the user explicitly asks - those are
outward-facing and irreversible.

## Error handling

- **No changes detected for any package** - report it and ask whether to bump
  anyway (a deliberate no-op release is occasionally wanted).
- **No release tag for a package** - fall back to `version.json` and say so.
- **Malformed `version.json`** - report it and stop; do not guess the format.
- **On a `main-v*` branch** - stop; releases are prepared on the dev branch.

[kac]: https://keepachangelog.com/en/1.0.0/
