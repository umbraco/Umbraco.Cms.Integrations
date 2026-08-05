---
name: post-release-cleanup
description: Closes out a release - merges the dev branch into its main-v<N> branch so it matches what shipped, then bumps the patch of each just-released package on dev so preview builds sort above the release. Use after a release has been published to NuGet and tagged.
---

# Post-Release Cleanup

You close out a release for **Umbraco.Cms.Integrations**.

## When to run

After `/release-management` prepared the bump **and** a human has:

1. Promoted the CI artifacts to NuGet, and
2. Created the `release/<slug>-<version>` tags.

If those have not happened yet, stop and say so - merging into `main-v<N>` before
the packages are actually published would leave `main-v<N>` claiming a release
that does not exist.

## Why the version bump matters

NBGV plus `Umbraco.GitVersioning.Extensions` produces preview versions like
`7.1.0--preview.4.gabc1234` on the dev branch. That sorts **below** the stable
`7.1.0` in SemVer, so without bumping the patch on dev after a release, dev builds
look older than what just shipped and are useless for testing.

## What this repo does NOT need

Adapted from `Umbraco.AI`'s skill of the same name. Two of its phases do not apply:

- **No release branch to merge or delete.** Bumps land directly on `v18/dev` /
  `v17/dev`, so the merge is simply dev → `main-v<N>`.
- **No major-version cutover.** A new Umbraco major gets a deliberately created
  `main-v<N>` / `v<N>/dev` pair as a planning decision. Never create those
  branches or change the repo's default branch from this skill.

---

## Phase 1: Work out what was released

```bash
git branch --show-current      # expect v18/dev or v17/dev
git fetch origin --tags
```

Derive the line from the branch (`v18/dev` → `v18`, so main is `main-v18`).

Find the release tags that are on this branch but not yet on `main-v<N>`:

```bash
merge_base=$(git merge-base origin/main-v18 HEAD)
for tag in $(git tag --list 'release/*'); do
  tag_commit=$(git rev-parse "$tag^{commit}")
  if git merge-base --is-ancestor "$tag_commit" HEAD 2>/dev/null && \
     ! git merge-base --is-ancestor "$tag_commit" origin/main-v18 2>/dev/null; then
    echo "$tag"
  fi
done
```

Parse each tag into a package and version. Map the slug back to the package folder
by matching against the discovered packages, rather than reversing the slug rules
(the historical slugs are not perfectly consistent).

Confirm with the user:

```
Released on v18/dev, not yet on main-v18:
  - Search.Algolia 7.1.0  (release/search-algolia-7.1.0)
  - Crm.Hubspot    9.0.2  (release/crm-hubspot-9.0.2)

Merge into main-v18 and bump dev patches? [Yes / Cancel]
```

**If no such tags exist**, warn and offer choices - do not just proceed:

```
No release tags found on this branch that are missing from main-v18.

That usually means the tags have not been created or pushed yet.

Options:
  - Create the tags first, then re-run
  - Merge only (skip the version bump)
  - Cancel
```

---

## Phase 2: Merge dev into main-v&lt;N&gt;

Confirm with the user before the first push - this updates a shared branch.

```bash
git checkout main-v18
git pull origin main-v18
git merge v18/dev --no-ff -m "Merge v18/dev into main-v18 for release"
git push origin main-v18
```

If there are conflicts, resolve with these rules and ask about anything else:

- `version.json` - keep the **higher** version.
- `CHANGELOG.md` - keep **both** sets of entries, newest version first.

Never force-push. If a push is rejected, pull and retry the merge.

---

## Phase 3: Bump the patch on dev

```bash
git checkout v18/dev
git pull origin v18/dev
```

For each package released in Phase 1, increment the patch in its `version.json`:

- `7.1.0` → `7.1.1`
- `9.0.2` → `9.0.3`
- Dotted pre-release `8.0.0-beta.2` → `8.0.0-beta.3`
- Pre-release with no number `8.0.0-alpha` → `8.0.0-alpha.1`

Always bump the **patch**, whatever kind of bump the release itself was. This is
mechanical - it is only there to keep preview builds sorting above the release.

> **Changelog check.** CI fails a version bump with no matching changelog entry.
> A bare patch bump here has no user-facing content to describe, so add a short
> `### Internal` entry for the new version, e.g.
> `* Development version bump after the 7.1.0 release.`
> Then run `pwsh -File .azure-pipelines/scripts/validate-changelogs.ps1` to confirm.

Commit and push:

```bash
git add src/*/version.json src/*/CHANGELOG.md
git commit -m "chore(release): Bump dev versions after release

- Umbraco.Cms.Integrations.Search.Algolia: 7.1.0 -> 7.1.1
- Umbraco.Cms.Integrations.Crm.Hubspot: 9.0.2 -> 9.0.3"
git push origin v18/dev
```

---

## Phase 4: Summary

```
Post-release cleanup complete.

Merged:
  v18/dev -> main-v18

Dev version bumps on v18/dev:
  - Search.Algolia: 7.1.0 -> 7.1.1
  - Crm.Hubspot:    9.0.2 -> 9.0.3

Preview builds on v18/dev now sort above the released versions.
```

Then remind the user of anything still outstanding - most often the GitHub release
for each tag, if that was not done before this ran.

## Error handling

- **Merge conflicts** - resolve per the rules in Phase 2, ask about the rest.
- **Push rejected** - the branch is probably protected, or someone else pushed.
  Pull and retry. Never force-push; report it if it cannot be resolved.
- **Malformed `version.json`** - show the user and ask; do not guess.
- **Not on a dev branch** - stop and say which branch to run from.
