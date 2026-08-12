---
name: post-release-cleanup
description: Closes out a release - merges the release branch into main-v<N> and back into v<N>/dev, bumps the patch of each released package on dev so preview builds sort above the release, and optionally deletes the release branch. Use after a release has been published to NuGet and tagged.
---

# Post-Release Cleanup

You close out a release for **Umbraco.Cms.Integrations**.

## Where this sits in the flow

```
1. /release-management        - cuts v17/release/YYYY.MM.N, writes
                               release-manifest.json, bumps version.json
                               + CHANGELOG there, pushes the branch
2. CI builds the release branch -> clean 7.1.0 artifacts
3. Promote to NuGet           (manual)
4. Tag release/<slug>-<version> (manual)
5. GitHub release per tag     (manual)
6. THIS SKILL                 - delete the manifest, merge back, bump dev
                               patches, tidy up
```

## When to run

Only after the packages are **actually on NuGet and tagged**. Merging into
`main-v<N>` before that would leave it claiming a release that does not exist.

## Why the version bump matters

NBGV plus `Umbraco.GitVersioning.Extensions` produces preview versions like
`7.1.0--preview.4.gabc1234` on `v17/dev`. That sorts **below** the stable `7.1.0`
in SemVer, so without bumping the patch on dev after a release, dev builds look
older than what just shipped and are useless for testing.

## What this repo does NOT need

Adapted from `Umbraco.AI`'s skill of the same name, minus:

- **Major-version cutover.** A new Umbraco major gets a deliberately created
  `main-v<N>` / `v<N>/dev` pair as a planning decision. Never create those branches
  or change the repo's default branch from this skill.
- **Git hooks.** `Umbraco.AI` deletes `release-manifest.json` with a `.githooks`
  post-merge hook. This repo has no `.githooks` directory, and adding one would
  mean every contributor has to set `core.hooksPath` or the cleanup silently stops
  happening. Phase 2 does it explicitly instead.

---

## Phase 1: Identify the release

```bash
git branch --show-current
git fetch origin --tags
```

Expect to be on `v<N>/release/*` or `v<N>/hotfix/*`. If not, ask the user which
release branch to process. Derive the line from the branch name
(`v17/release/2026.08.1` → line `v17`, released branch `main-v17`, dev `v17/dev`).

Find the release tags on this branch that are not yet on `main-v<N>`:

```bash
release_branch=$(git branch --show-current)

for tag in $(git tag --list 'release/*'); do
  tag_commit=$(git rev-parse "$tag^{commit}")
  if git merge-base --is-ancestor "$tag_commit" HEAD 2>/dev/null && \
     ! git merge-base --is-ancestor "$tag_commit" origin/main-v17 2>/dev/null; then
    echo "$tag"
  fi
done
```

Map each tag back to its package folder by matching against the discovered packages,
not by reversing the slug rules - the historical slugs are inconsistent (both
`crm-activecampaign` and `crm-active-campaign` exist).

Cross-check the tag versions against the `version.json` values on this branch. If
they disagree, stop and report it: it means what was tagged is not what this branch
builds.

Confirm with the user:

```
Release branch: v17/release/2026.08.1
Tagged and published:
  - Search.Algolia 7.1.0  (release/search-algolia-7.1.0)
  - Crm.Hubspot    9.0.2  (release/crm-hubspot-9.0.2)

Plan:
  1. merge v17/release/2026.08.1 -> main-v17
  2. merge v17/release/2026.08.1 -> v17/dev
  3. bump on v17/dev: 7.1.0 -> 7.1.1, 9.0.2 -> 9.0.3

Proceed? [Yes / Cancel]
```

**If no tags are found**, stop and offer choices - do not proceed:

```
No release tags found on this branch that are missing from main-v17.

That usually means the packages have not been tagged (or pushed) yet.

Options:
  - Tag and publish first, then re-run
  - Tell me which packages and versions were released, and I will use those
  - Merge only, and skip the version bump
  - Cancel
```

---

## Phase 2: Delete the manifest, then merge into main-v&lt;N&gt;

`release-manifest.json` belongs only to the release branch. It is **required** on
`v<N>/release/*`, so leaving it in place would carry it onto `main-v<N>` and
`v<N>/dev`, where it is meaningless and where the next release branch would
inherit a stale copy naming last release's packages.

Delete it on the release branch first, so both merges are clean:

```bash
git checkout "$release_branch"
git rm release-manifest.json
git commit -m "chore(release): Remove release manifest after publishing"
git push origin "$release_branch"
```

If the file is already absent, say so and move on - a hotfix release may not have
had one.

Then merge. Confirm before the first push - this updates a shared branch.

```bash
git checkout main-v17
git pull origin main-v17
git merge "$release_branch" --no-ff -m "Merge $release_branch into main-v17"
git push origin main-v17
```

`main-v17` now matches what shipped.

---

## Phase 3: Merge into v&lt;N&gt;/dev

The release branch carries the bumped `version.json` and changelog entries, so dev
needs them too or the next release starts from stale versions.

```bash
git checkout v17/dev
git pull origin v17/dev
git merge "$release_branch" --no-ff -m "Merge $release_branch into v17/dev"
```

Resolve conflicts as:

- `version.json` - keep the **higher** version (Phase 4 overwrites it anyway).
- `CHANGELOG.md` - keep **both** sets of entries, newest version first.
- `release-manifest.json` - delete it. It must not exist on dev. Phase 2 should
  have removed it already, so seeing it here means that step was skipped.
- Anything else - ask the user.

Never force-push. If a push is rejected, pull and retry the merge.

---

## Phase 4: Bump the patch on dev

For each released package, increment the patch in `src/<PackageName>/version.json`:

- `7.1.0` → `7.1.1`
- `9.0.2` → `9.0.3`
- Dotted pre-release `8.0.0-beta.2` → `8.0.0-beta.3`
- Pre-release with no number `8.0.0-alpha` → `8.0.0-alpha.1`

Always bump the **patch**, whatever kind of bump the release itself was. This is
mechanical - it only exists to keep preview builds sorting above the release.

**Do not touch `Client/public/umbraco-package.json`.** It is a fixed `1.0.0`
placeholder; the real version is stamped into the `wwwroot` copy by
`UpdatePackageManifestVersion` on CI builds. Bumping it here used to be required,
and it is what made three packages look changed to release detection.

> **Changelog gate.** CI fails a version bump with no matching changelog entry. A
> bare patch bump has no user-facing content, so add a short entry:
> ```markdown
> ## [7.1.1] - Unreleased
>
> ### Internal
>
> * Development version bump after the 7.1.0 release.
> ```
> Keep the literal word `Unreleased` and the `### Internal` bullet.
> `/release-management` finalises this same entry at the next release, replacing
> the word with a date and dropping the bullet - it does not add a second heading.
>
> Then confirm with:
> ```bash
> pwsh -File .azure-pipelines/scripts/validate-changelogs.ps1
> ```

Commit and push:

```bash
git add src/*/version.json src/*/CHANGELOG.md
git commit -m "chore(release): Bump dev versions after release 2026.08.1

- Umbraco.Cms.Integrations.Search.Algolia: 7.1.0 -> 7.1.1
- Umbraco.Cms.Integrations.Crm.Hubspot: 9.0.2 -> 9.0.3"
git push origin v17/dev
```

---

## Phase 5: Delete the release branch (optional)

Ask the user:

```
Delete v17/release/2026.08.1?
  - Local and remote
  - Local only
  - Keep it
```

The tags are what preserve the released history, so deleting the branch loses
nothing. Only delete it once both merges are pushed.

```bash
git branch -d v17/release/2026.08.1
git push origin --delete v17/release/2026.08.1
```

Use `-d`, not `-D`: if git refuses because the branch is unmerged, that is a real
signal one of the merges did not land - investigate rather than forcing it.

---

## Phase 6: Summary

```
Post-release cleanup complete.

Merged:
  v17/release/2026.08.1 -> main-v17
  v17/release/2026.08.1 -> v17/dev

Dev version bumps on v17/dev:
  - Search.Algolia: 7.1.0 -> 7.1.1
  - Crm.Hubspot:    9.0.2 -> 9.0.3

Release branch: deleted / kept

Preview builds on v17/dev now sort above the released versions.
```

Then flag anything still outstanding - most often a missing GitHub release for one
of the tags.

## Error handling

- **Merge conflicts** - resolve per the rules in Phase 3, ask about the rest.
- **Push rejected** - the branch is protected, or someone else pushed. Pull and
  retry. Never force-push.
- **Tag versions disagree with `version.json`** - stop. What was published may not
  match this branch; that needs a human decision.
- **Malformed `version.json`** - show the user and ask; do not guess.
