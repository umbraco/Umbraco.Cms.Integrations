# Changelog - Umbraco.Cms.Integrations.Search.Algolia

All notable changes to Umbraco.Cms.Integrations.Search.Algolia will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [6.1.2] - 2026-08-11

### fix

* Skip records Algolia rejects instead of abandoning the index build. An index build now pushes in chunks and retries a rejected chunk one record at a time, so a single oversized record no longer leaves the rest of the site unindexed ([dafdcf0](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/dafdcf0))
* Remove descendants from the Algolia index when a branch is deleted. Deleting a parent previously left every descendant behind in the index ([67f9304](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/67f9304))

## [6.1.1] - 2026-08-10

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
