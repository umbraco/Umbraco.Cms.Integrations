# Changelog - Umbraco.Cms.Integrations.Analytics.Cookiebot

All notable changes to Umbraco.Cms.Integrations.Analytics.Cookiebot will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.0.0] - 2026-08-11

### Breaking

* Requires Umbraco CMS 18. The package targets `net10.0` and depends on
  `Umbraco.Cms.Web.Website [18.0.0, 19.0.0)`. Use the 2.x line for Umbraco 15-16.
* The major version now tracks the Umbraco major, matching the other packages in
  this repository, so this release jumps from 2.x to 4.x on the v18 line (3.x is
  the v17 line).

### Internal

* Brought forward from the `main-v15` branch, where the package was last
  maintained at 2.0.1. The Cookiebot banner and declaration partial views are
  unchanged; they only use `IConfiguration`, so no CMS API updates were needed.

## [2.0.0] - 2024-12-02

### Internal

* Last released version of the 2.x line, supporting Umbraco 15-16. Recorded here
  as the baseline for this changelog; earlier history is in the git history and
  the GitHub releases.
