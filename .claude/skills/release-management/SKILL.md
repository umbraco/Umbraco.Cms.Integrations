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
- **`release-manifest.json` decides what ships.** As in `Umbraco.AI`. It is
  required on `v<N>/release/*`, optional on `v<N>/hotfix/*`, and its `include`
  list replaces the build set outright. Change detection in
  `detect-changes.ps1` is only a cross-check: anything it finds changed must
  appear in `include` or `exclude`, or CI fails.

  A bumped `version.json` is **not** the declaration, and selecting on it does
  not work. `/post-release-cleanup` bumps each released package's patch on dev,
  which used to make every previously-released package look like it was shipping
  on every subsequent release branch, while a package whose version had never
  moved could not be selected at all.
- **The version is usually already bumped.** Because of that same dev patch bump,
  the target version is often already in `version.json` when you cut the branch.
  Check before editing.
- **`Client/public/umbraco-package.json` is a fixed `1.0.0` placeholder.** Do not
  bump it. The real version is stamped into the `wwwroot` copy by CI. Editing it
  would also make the package look changed to release detection.
- **Tags are `release/<slug>-<version>`**, e.g. `release/search-algolia-7.1.0` -
  not `Product@Version`. Note the branch is `v18/release/<date>` while tags are
  `release/<slug>-<version>`; they live in different ref namespaces and do not
  collide.
- **`main-v<N>`, not `v<N>/main`.** This repo names its released branch
  `main-v18`; `Umbraco.AI`/`Umbraco.Automate` use `v18/main`. The
  `publicReleaseRefSpec` entries reflect that difference.

## Task

1. Detect which packages changed since their last release tag.
2. Recommend a version bump per package from the commit history.
3. Confirm with the user.
4. **Cut the release branch** - before any file changes.
5. Write `release-manifest.json`.
6. Update each package's `version.json`, where it is not already correct.
7. Finalise each package's `CHANGELOG.md` entry.
8. Review the changelogs for noise and completeness.
9. Validate, dry-run the selection, commit and push the release branch.
10. Report what remains manual.

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

## Phase 4: Cut the release branch

**Do this before touching any file.** The bump belongs on the release branch, not
on `v18/dev`.

Why it matters: `version.json` sets `publicReleaseRefSpec` to `main-v18`,
`v18/release/*` and `v18/hotfix/*`. NBGV only drops the preview suffix on a branch
matching one of those:

| Branch built | Version produced |
|---|---|
| `v18/release/2026.08.1` | `7.1.0` ← publishable |
| `main-v18` | `7.1.0` |
| `v18/dev`, feature branches | `7.1.0--preview.4.gabc1234` |

So the release branch is what produces the artifact you publish. This is the same
arrangement `Umbraco.AI` and `Umbraco.Automate` use.

**Naming:** `v<N>/release/YYYY.MM.N` - calendar-based, matching the sibling repos.
It is independent of the package versions: one release branch can carry several
packages at different versions.

**`N` counts release events across the whole repo, not per line.** It comes from
the date tags, which have no line prefix, so a v17 release and a v18 release share
one sequence. `Umbraco.Automate` and `Umbraco.AI` both work this way - Automate
currently has `v18/release/2026.08.1` alongside `v17/release/2026.08.2`, because
the v18 release took `.1` and the v17 one that followed took `.2`.

Do **not** derive `N` from branch names. That gives both lines a `.1` in the same
month, and the collision only shows up later when the date tags are created.

Work out the name:

```bash
git branch --show-current            # the line prefix, e.g. v18/dev -> v18
date +%Y.%m                          # e.g. 2026.08

git fetch origin --tags
git tag --list "2026.08.*" | sort -V | tail -1   # highest release event this month
```

`git tag --list "2026.08.*"` matches only date tags, never the
`release/<slug>-<version>` package tags. Take the highest trailing number and add
one; start at `1` if the month has none. Confirm the name with the user, then:

```bash
git checkout -b v18/release/2026.08.1
```

The matching date tag `2026.08.1` is created at release time, in Phase 10, next to
the per-package tags. It is what lets the next release pick the right number, so
skipping it breaks the sequence for whoever releases next - on either line.

For an urgent fix on top of an already-released state, cut
`v18/hotfix/YYYY.MM.N` from `main-v18` instead of from dev.

---

## Phase 5: Write the release manifest

Still on the release branch, before touching versions. **This is what CI treats as
the decision on what ships** - see `Get-ManifestSelection` in
`.azure-pipelines/scripts/detect-changes.ps1`. Change detection there is only a
cross-check.

```bash
pwsh -File scripts/generate-release-manifest.ps1 \
  -Include Umbraco.Cms.Integrations.Crm.Dynamics
```

Full package folder names. The script validates them against the real package
list, so a typo fails here rather than in CI, and it is case-insensitive so you do
not have to remember that git tracks `...URLInspectionTool`.

If any package has substantive changes but is deliberately **not** shipping, name
it in `-Exclude`. CI fails when a changed package appears in neither list - that
guard is the whole point, so do not work around it by dropping the package from
the release without saying so.

`release-manifest.json` is **required** on `v<N>/release/*` and optional on
`v<N>/hotfix/*`. `/post-release-cleanup` deletes it before merging back, so it
never reaches `main-v<N>` or `v<N>/dev`.

---

## Phase 6: Update the version files

For each confirmed package, edit the `version` field of
`src/<PackageName>/version.json`. Leave every other property untouched. This is
the only file that drives the version.

**Usually it is already correct.** `/post-release-cleanup` bumps each released
package's patch on dev after a release, so the target version is often already
sitting there. Check before editing.

Do **not** touch:

- the `.csproj` (it has no `<Version>` - the version comes from `version.json`)
- `Client/public/umbraco-package.json` - it is a fixed `1.0.0` placeholder and is
  no longer maintained by hand. The real version is stamped into the `wwwroot`
  copy by `UpdatePackageManifestVersion` in the root `Directory.Build.targets`.
  Bumping it would also make the package look changed to release detection.
- `wwwroot/umbraco-package.json` - build output, stamped on CI builds, so any
  value committed here is overwritten anyway

---

## Phase 7: Write the changelog entry

**Check for an existing entry first.** `/post-release-cleanup` leaves an
`## [<version>] - Unreleased` heading on dev for the version being worked on, so
the entry for the version you are releasing usually already exists. **Finalise it
- do not add a second one**, or the changelog ends up with two headings for the
same version.

Finalising means:

- Replace `Unreleased` with today's real date. Get it with `date +%Y-%m-%d` - do
  not guess.
- Delete the `### Internal` / "Development version bump after the X release."
  bullet. That is bookkeeping, not a released change.
- Add the real bullets underneath.

Only write a fresh entry when there is genuinely no heading for that version.
Either way the result is one entry, in [Keep a Changelog][kac] format:

```markdown
## [7.1.0] - 2026-08-12

### feat

* **algolia:** Add support for filtered replica indices ([abc1234](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/abc1234))

### fix

* **algolia:** Refresh the content cache for changed nodes ([def5678](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/def5678))
```

Rules:

- Group by commit type, using the type as the heading (`feat`, `fix`, `perf`).
- One bullet per user-facing change, taken from the commit subject.
- A breaking change gets its own `### Breaking` section, listed first.

---

## Phase 8: Review the changelogs

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


## Phase 9: Validate, dry-run, commit and push the release branch

Run the same changelog check CI runs, so a failure surfaces here rather than in
the pipeline:

```bash
pwsh -File .azure-pipelines/scripts/validate-changelogs.ps1
```

**Then dry-run the selection.** There are no automated tests for the selection
logic, so this is the only check that what ships is what you meant:

```bash
pwsh -File .azure-pipelines/scripts/detect-changes.ps1 \
  -SourceBranch "refs/heads/v18/release/2026.08.1" \
  -ReleaseCompareRef "origin/main-v18"
```

Read the `Build plan:` block and confirm the `BUILD` lines are exactly the
packages you intend to publish. Watch for two things:

- A `note: <package> has no substantive changes but is being released` line.
  Legitimate for a package that has never shipped, suspicious otherwise.
- A throw about a changed package missing from the manifest. Add it to `include`
  or `exclude` - do not delete the guard.

Show the user that block before pushing.

Then commit and push:

```bash
git add release-manifest.json src/*/version.json src/*/CHANGELOG.md
git commit -m "chore(release): Prepare release 2026.08.1

- Umbraco.Cms.Integrations.Search.Algolia: 7.0.1 -> 7.1.0
- Umbraco.Cms.Integrations.Crm.Hubspot: 9.0.1 -> 9.0.2"

git push -u origin v18/release/2026.08.1
```

`release-manifest.json` must be in the commit, or CI fails on the release branch.
Do not add `src/*/Client/public/umbraco-package.json` - it is a placeholder now
and a release does not touch it.

Use the **release branch name** in the commit subject, not the package versions -
one branch can carry several packages at different versions.

Pushing a new release branch is safe: it touches no shared branch, and nothing is
published until a human promotes the artifact. `main-v18` and `v18/dev` are left
untouched until `/post-release-cleanup` runs.

CI will then build this branch and produce **clean-versioned** artifacts, for
exactly the packages named in the manifest's `include` list.

---

## Phase 10: Report what happens next

```
Release branch v18/release/2026.08.1 pushed, carrying:
  - Search.Algolia 7.0.1 -> 7.1.0
  - Crm.Hubspot    9.0.1 -> 9.0.2

main-v18 and v18/dev are untouched.

Next:
  1. Wait for CI on v18/release/2026.08.1. Confirm the artifacts are
     clean-versioned (7.1.0, NOT 7.1.0--preview.N) before going further.
  2. Promote each artifact to NuGet. This is the one step no skill does.
  3. Run /post-release-cleanup on this branch. It verifies the packages are
     on NuGet, creates the tags and the GitHub releases, deletes the manifest,
     merges into main-v18 and v18/dev, and bumps the dev patches.
```

**Do not tag, and do not create GitHub releases here.** They belong to
`/post-release-cleanup`, and the reason is timing, not squeamishness: at this point
CI has not built anything, so nothing is published for a tag to point at. A tag
created now becomes a lie the moment someone decides not to promote the artifact.

That also means **do not** offer to tag as a convenience if the user asks "what's
left" - point them at `/post-release-cleanup` instead, so the tag, the GitHub
release and the merge-back all happen from one place with one confirmation.

> The two tag shapes answer different questions. `release/<slug>-<version>` is
> "which commit is this published package", one per package. `YYYY.MM.N` is "which
> commits went out together", one per release branch. They share the tag namespace
> but cannot collide: one starts with `release/`, the other with a digit.

## Error handling

- **No changes detected for any package** - report it and ask whether to bump
  anyway (a deliberate no-op release is occasionally wanted).
- **No release tag for a package** - fall back to `version.json` and say so.
- **Malformed `version.json`** - report it and stop; do not guess the format.
- **Started from `main-v*`** - a normal release is cut from `v<N>/dev`. Only a
  hotfix is cut from `main-v<N>`; confirm which the user means.
- **A release branch for this month already exists** - it may be an in-flight
  release. Show it and ask whether to add to it or start the next number.
- **The dry-run throws about an unaccounted changed package** - a package has
  substantive changes and is in neither `include` nor `exclude`. Decide which it
  belongs in with the user and regenerate the manifest. Never silence the guard.
- **The dry-run notes a package with no substantive changes** - fine for a
  package that has never shipped, or when rebuilding an already merged-back
  release branch. Otherwise it is probably in `include` by mistake.

[kac]: https://keepachangelog.com/en/1.0.0/
