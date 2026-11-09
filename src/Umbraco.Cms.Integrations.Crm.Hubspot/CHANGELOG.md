# Changelog - Umbraco.Cms.Integrations.Crm.Hubspot

All notable changes to Umbraco.Cms.Integrations.Crm.Hubspot will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [10.0.0] - Unreleased

### Breaking

* Requires Umbraco CMS 19 (built against 19.0.0-beta1) and .NET 11. The package targets `net11.0` and depends on `Umbraco.Cms.*` `[19.0.0-beta1, 20.0.0)`. Use the 9.x line for Umbraco 18.

### Internal

* The backoffice client now authenticates with the Umbraco 19 cookie (`authContext.configureClient`) instead of a bearer token, and is built against `@umbraco-cms/backoffice` 19.0.0-beta1.
* Form picker value converters use `PropertyCacheLevel.Element` instead of the obsolete `Snapshot`; the converted value depends only on the stored value, so behaviour is unchanged.

## [9.0.2] - 2026-08-11

### fix

* Show the correct configuration description in the form picker's authorization panel, which reported "No API or OAuth configuration could be found" even on a correctly configured site ([0f031e3](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/0f031e3))

## [9.0.1] - 2026-07-16

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
