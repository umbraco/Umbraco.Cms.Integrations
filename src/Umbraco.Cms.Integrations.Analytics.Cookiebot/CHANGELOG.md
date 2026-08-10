# Changelog - Umbraco.Cms.Integrations.Analytics.Cookiebot

All notable changes to Umbraco.Cms.Integrations.Analytics.Cookiebot will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - Unreleased

### Breaking

* Requires Umbraco CMS 17. The package targets `net10.0` and depends on
  `Umbraco.Cms.Web.Website [17.0.0, 18.0.0)`. Use the 2.x line for Umbraco 15-16.
* The major version now tracks the Umbraco major, matching the other packages in
  this repository, so this release jumps from 2.x to 3.x on the v17 line (4.x is
  the v18 line).

### Internal

* Brought onto the v17 line from the `main-v15` branch, where the package was
  last maintained at 2.0.1. The Cookiebot banner and declaration partial views
  are unchanged; they only use `IConfiguration`, so no CMS API updates were
  needed.

## [2.0.0] - 2024-12-02

### Internal

* Last released version of the 2.x line, supporting Umbraco 15-16. Recorded here
  as the baseline for this changelog; earlier history is in the git history and
  the GitHub releases.
