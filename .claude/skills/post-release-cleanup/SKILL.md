---
name: post-release-cleanup
description: Closes out a release by bumping the patch of each just-released package on the dev branch, so preview builds sort above the release. Use after a release has been published to NuGet and tagged. The merge into main-v<N> is not done here - it happens during /release-management, before publishing.
---

# Post-Release Cleanup

You close out a release for **Umbraco.Cms.Integrations**.

## Where this sits in the flow

```
1. /release-management on v18/dev   - bump version.json + CHANGELOG, push
2. /release-management merges v18/dev -> main-v18   (still part of that skill)
3. CI builds main-v18              -> clean 7.1.0 artifacts
4. Promote to NuGet                (manual)
5. Tag release/<slug>-<version>    (manual)
6. GitHub release per tag          (manual)
7. THIS SKILL                      - bump dev patch versions
```

**The merge into `main-v18` is not this skill's job.** It has to happen at step 2,
before publishing, because `publicReleaseRefSpec` is `^refs/heads/main-v18$` and
only a `main-v18` build produces a clean, publishable version. If you find
`main-v18` does not yet contain the release, something went wrong earlier - say so
rather than merging here.

## Why the version bump matters

NBGV plus `Umbraco.GitVersioning.Extensions` produces preview versions like
`7.1.0--preview.4.gabc1234` on the dev branch. That sorts **below** the stable
`7.1.0` in SemVer, so without bumping the patch on dev after a release, dev builds
look older than what just shipped and are useless for testing.

## What this repo does NOT need

Adapted from `Umbraco.AI`'s skill of the same name, minus:

- **Release branches.** Bumps land on `v18/dev` and are merged to `main-v18`; there
  is no `vN/release/*` branch to merge or delete.
- **Major-version cutover.** A new Umbraco major gets a deliberately created
  `main-v<N>` / `v<N>/dev` pair as a planning decision. Never create those branches
  or change the repo's default branch from this skill.

---

## Phase 1: Confirm the release actually shipped

```bash
git branch --show-current      # expect v18/dev or v17/dev
git fetch origin --tags
```

Derive the line from the branch (`v18/dev` → main is `main-v18`).

Find the release tags for this release - they should be on `main-v18`:

```bash
for tag in $(git tag --list 'release/*'); do
  tag_commit=$(git rev-parse "$tag^{commit}")
  if git merge-base --is-ancestor "$tag_commit" origin/main-v18 2>/dev/null; then
    echo "$tag"
  fi
done | tail -20
```

Identify which of those tags are new since the last cleanup - the ones whose
version matches the current `version.json` of a package on this branch. Map a slug
back to its package folder by matching against the discovered packages, not by
reversing the slug rules (the historical slugs are inconsistent).

**Two things to verify before bumping anything:**

1. **The tags exist.** If not, the release has not been tagged yet.
2. **`main-v18` contains the bump.** Check with:
   ```bash
   git merge-base --is-ancestor HEAD origin/main-v18 && echo "dev is merged" || echo "dev is AHEAD of main-v18"
   ```
   Some dev commits being ahead is normal if work continued after the release. What
   matters is that the release commit itself is on `main-v18`.

Confirm with the user:

```
Released and tagged on main-v18:
  - Search.Algolia 7.1.0  (release/search-algolia-7.1.0)
  - Crm.Hubspot    9.0.2  (release/crm-hubspot-9.0.2)

Bump these to 7.1.1 and 9.0.3 on v18/dev? [Yes / Cancel]
```

**If no release tags are found**, stop and offer choices - do not proceed:

```
No new release tags found on main-v18.

That usually means the packages have not been published and tagged yet,
or the merge into main-v18 did not happen.

Options:
  - Finish publishing and tagging first, then re-run
  - Tell me which packages and versions were released, and I will bump those
  - Cancel
```

---

## Phase 2: Bump the patch on dev

```bash
git checkout v18/dev
git pull origin v18/dev
```

For each released package, increment the patch in `src/<PackageName>/version.json`:

- `7.1.0` → `7.1.1`
- `9.0.2` → `9.0.3`
- Dotted pre-release `8.0.0-beta.2` → `8.0.0-beta.3`
- Pre-release with no number `8.0.0-alpha` → `8.0.0-alpha.1`

Always bump the **patch**, whatever kind of bump the release itself was. This is
mechanical - it only exists to keep preview builds sorting above the release.

> **Changelog gate.** CI fails a version bump with no matching changelog entry. A
> bare patch bump has no user-facing content, so add a short entry for the new
> version:
> ```markdown
> ## [7.1.1] - Unreleased
>
> ### Internal
>
> * Development version bump after the 7.1.0 release.
> ```
> Then confirm with:
> ```bash
> pwsh -File .azure-pipelines/scripts/validate-changelogs.ps1
> ```

Commit and push:

```bash
git add src/*/version.json src/*/CHANGELOG.md
git commit -m "chore(release): Bump dev versions after release

- Umbraco.Cms.Integrations.Search.Algolia: 7.1.0 -> 7.1.1
- Umbraco.Cms.Integrations.Crm.Hubspot: 9.0.2 -> 9.0.3"
git push origin v18/dev
```

---

## Phase 3: Summary

```
Post-release cleanup complete.

Dev version bumps on v18/dev:
  - Search.Algolia: 7.1.0 -> 7.1.1
  - Crm.Hubspot:    9.0.2 -> 9.0.3

Preview builds on v18/dev now sort above the released versions.
```

Then flag anything still outstanding - most often a missing GitHub release for one
of the tags.

## Error handling

- **`main-v18` is missing the release** - stop and report it. Do not merge here;
  that belongs to `/release-management` before publishing. Merging after the fact
  means the published artifact was built from the wrong branch and is probably
  preview-versioned, which needs a human decision.
- **Push rejected** - the branch is protected, or someone else pushed. Pull and
  retry. Never force-push.
- **Malformed `version.json`** - show the user and ask; do not guess.
- **Not on a dev branch** - stop and say which branch to run from.
