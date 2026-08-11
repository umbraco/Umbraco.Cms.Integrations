# Changelog - Umbraco.Cms.Integrations.Search.Algolia

All notable changes to Umbraco.Cms.Integrations.Search.Algolia will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [7.0.2] - 2026-08-11

### fix

* Skip records Algolia rejects instead of abandoning the index build. An index build now pushes in chunks and retries a rejected chunk one record at a time, so a single oversized record no longer leaves the rest of the site unindexed ([bd806b5](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/bd806b5))
* Remove descendants from the Algolia index when a branch is deleted. Deleting a parent previously left every descendant behind in the index ([6e6fdab](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/6e6fdab))

## [7.0.1] - 2026-07-16

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
