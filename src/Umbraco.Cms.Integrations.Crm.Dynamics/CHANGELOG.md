# Changelog - Umbraco.Cms.Integrations.Crm.Dynamics

All notable changes to Umbraco.Cms.Integrations.Crm.Dynamics will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [6.0.3] - Unreleased

### Internal

* Development version bump after the 6.0.2 release.

## [6.0.2] - 2026-08-11

### fix

* Skip the access token column resize on SQLite. The migration threw `NotSupportedException` on every startup against SQLite and left the migration plan permanently unfinished ([8cdaf6d](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/8cdaf6d))
* Select a form when clicking anywhere on its row in the form picker. Only a click on the form name registered before, so most of the row highlighted without recording a choice ([b87acf2](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/b87acf2))

## [6.0.1] - 2026-07-16

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
