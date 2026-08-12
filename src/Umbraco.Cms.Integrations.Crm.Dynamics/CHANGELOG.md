# Changelog - Umbraco.Cms.Integrations.Crm.Dynamics

All notable changes to Umbraco.Cms.Integrations.Crm.Dynamics will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.0.4] - Unreleased

### Internal

* Development version bump after the 5.0.3 release.

## [5.0.3] - 2026-08-12

### fix

* Render a real-time form in the iframe when Dynamics returns no `data-cached-form-url`. The standalone URL was empty in that case, so the iframe loaded nothing; the form HTML is now embedded via `srcdoc` and falls back to the raw form HTML when no standalone HTML is returned. Reported in [#257](https://github.com/umbraco/Umbraco.Cms.Integrations/pull/257)

## [5.0.2] - 2026-08-11

### fix

* Skip the access token column resize on SQLite. The migration threw `NotSupportedException` on every startup against SQLite and left the migration plan permanently unfinished ([e0a9b0a](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/e0a9b0a))
* Select a form when clicking anywhere on its row in the form picker. Only a click on the form name registered before, so most of the row highlighted without recording a choice ([69f92ef](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/69f92ef))

## [5.0.1] - 2026-08-10

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
