# Changelog - Umbraco.Cms.Integrations.Automation.Zapier

All notable changes to Umbraco.Cms.Integrations.Automation.Zapier will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.0.2] - Unreleased

### Internal

* Development version bump after the 5.0.1 release.

## [5.0.1] - 2026-11-26

### fix

* **zapier:** Signing in with a username and password no longer fails with a NullReferenceException when `UserGroupAlias` is set ([db014a0](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/db014a0))
* **zapier:** More than one Zap can subscribe to the same content type or form. A migration replaces the unique index on `EntityId, Type` with a non-unique one; `HookUrl` stays unique ([42b50a3](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/42b50a3))

## [5.0.0] - 2026-08-10

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
