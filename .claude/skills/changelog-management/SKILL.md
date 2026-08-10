---
name: changelog-management
description: Generates and previews per-package CHANGELOG.md entries from conventional commit history. Use when previewing what a package would release, backfilling a missing entry, or fixing a changelog CI failure - outside of a full release.
allowed-tools: Bash, Read, Edit, AskUserQuestion
---

# Changelog Manager

Generates and previews `CHANGELOG.md` entries for the packages in this repo, from
conventional commit history.

`/release-management` already writes changelog entries as part of cutting a
release. Use this skill when you want the changelog work on its own:

- previewing what a package *would* release, before deciding to release it
- backfilling an entry for a version that shipped without one
- fixing a `validate-changelogs.ps1` CI failure
- rewriting an entry that reads badly

## What makes this repo different

- **No generation script.** `Umbraco.Automate` and `Umbraco.AI` have
  `scripts/generate-changelog.js` behind `npm run changelog`. This repo has no
  such script, so entries come from reading `git log` directly. Do not invent a
  script or an npm target - neither exists.
- **One changelog per package**, at `src/<PackageName>/CHANGELOG.md`. There is no
  repo-level changelog.
- **Packages are independent.** A commit only belongs in a package's changelog if
  it touched that package's folder. No cascade, no shared entries.

## Format

[Keep a Changelog][kac], matching what every package already has:

```markdown
## [7.1.0] - 2026-08-12

### feat

* **algolia:** Add support for filtered replica indices ([abc1234](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/abc1234))

### fix

* **algolia:** Refresh the content cache for changed nodes ([def5678](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/def5678))
```

Rules:

- New entries go at the top, directly below the header block.
- Get the date with `date +%Y-%m-%d`. Never guess it.
- Group by commit type, using the type as the heading: `feat`, `fix`, `perf`.
- A breaking change gets its own `### Breaking` section, listed first.
- One bullet per user-facing change, from the commit subject.
- `docs:`, `chore:`, `refactor:`, `test:` and `ci:` are normally left out. If a
  version contains nothing else, use `### Internal` and say so in one line.

## Phase 1: Pick the package and the range

```bash
ls -d src/Umbraco.Cms.Integrations.*/          # the packages
cat src/<PackageName>/version.json             # current version
git tag --list 'release/*' | sort -V | tail    # confirm the real tag slug
```

> Tag slugs are **not** consistent in this repo's history - both
> `crm-activecampaign` and `crm-active-campaign` exist, and Shopify has both
> `shopify-1.2.0` and `commerce-shopify-*`. Always confirm against
> `git tag --list`; never derive the slug from the package name.

Range is that package's last release tag to `HEAD`. With no tag, fall back to the
last commit that changed its `CHANGELOG.md`, and say which you used.

## Phase 2: Read the commits

```bash
git log <lastTag>..HEAD --format='%h %s' -- src/<PackageName>/
```

Path-scoped, so only commits touching that package appear. Note that a
`Directory.Packages.props` change will not show up here even though it can affect
the package - mention it separately if the range contains one.

## Phase 3: Write or preview

State clearly which you are doing.

**Preview** - print the proposed entry, change nothing.

**Write** - insert the entry below the header block with `Edit`. Never reorder or
reword existing entries; only add.

If the version already has an entry, ask before touching it. An entry for a
shipped version is a historical record.

## Phase 4: Validate

```bash
pwsh -File .azure-pipelines/scripts/validate-changelogs.ps1
```

This is the same check CI runs. It fails when a `version.json` moved with no
matching `## [<version>]` heading, so it catches a bumped-but-undocumented
package - which is exactly the CI failure this skill is usually invoked to fix.

## Do not

- Commit or push unless asked. `/release-management` owns the release commit.
- Bump `version.json`. That is a release decision, not a changelog one.
- Write entries for packages the range did not touch.

## Error handling

- **No commits in range** - say so. Do not invent an entry.
- **Commits are not conventional** - group by reading the subjects, and say the
  grouping was inferred rather than parsed.
- **No tag for the package** - state the fallback you used for the range.
- **Version already documented** - stop and ask.

[kac]: https://keepachangelog.com/en/1.0.0/
