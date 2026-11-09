# Changelog - Umbraco.Cms.Integrations.SEO.Semrush

All notable changes to Umbraco.Cms.Integrations.SEO.Semrush will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [6.0.0] - Unreleased

### Breaking

* Requires Umbraco CMS 19 (built against 19.0.0-beta1) and .NET 11. The package targets `net11.0` and depends on `Umbraco.Cms.*` `[19.0.0-beta1, 20.0.0)`. Use the 5.x line for Umbraco 18.

### Internal

* The backoffice client now authenticates with the Umbraco 19 cookie (`authContext.configureClient`) instead of a bearer token, and is built against `@umbraco-cms/backoffice` 19.0.0-beta1.


## [5.0.0] - 2026-06-23

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
