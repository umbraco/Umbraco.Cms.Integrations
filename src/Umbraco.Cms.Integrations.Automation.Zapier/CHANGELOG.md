# Changelog - Umbraco.Cms.Integrations.Automation.Zapier

All notable changes to Umbraco.Cms.Integrations.Automation.Zapier will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [6.0.1] - Unreleased

### fix

* **zapier:** Signing in with a username and password no longer fails with a NullReferenceException when `UserGroupAlias` is set ([e47810c](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/e47810c))
* **zapier:** More than one Zap can subscribe to the same content type or form. A migration replaces the unique index on `EntityId, Type` with a non-unique one; `HookUrl` stays unique ([eb660d0](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/eb660d0))

## [6.0.0] - 2026-06-23

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
