# Changelog - Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool

All notable changes to Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.0.0] - Unreleased

### Breaking

* Requires Umbraco CMS 19 (built against 19.0.0-beta1) and .NET 11. The package targets `net11.0` and depends on `Umbraco.Cms.*` `[19.0.0-beta1, 20.0.0)`. Use the 3.x line for Umbraco 18.

### Internal

* The backoffice client now authenticates with the Umbraco 19 cookie (`authContext.configureClient`) instead of a bearer token, and is built against `@umbraco-cms/backoffice` 19.0.0-beta1.


## [3.0.0] - Unreleased

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json`
  versioning. Unlike the other packages, no `release/` tag exists for this
  version, so it is recorded as unreleased rather than given a release date.
  Changes before this version are in the git history.
