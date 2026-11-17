# Changelog - Umbraco.Cms.Integrations.Automation.Zapier

All notable changes to Umbraco.Cms.Integrations.Automation.Zapier will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [7.0.0-rc1] - Unreleased

### Breaking

* Requires Umbraco CMS 19 (built against 19.0.0-rc1) and .NET 11. The package targets `net11.0` and depends on `Umbraco.Cms.*` `[19.0.0-rc1, 20.0.0)`. Use the 6.x line for Umbraco 18.

### fix

* Keep sending payloads for single media pickers and single dropdowns now that Umbraco 19 moves them to `Umbraco.MediaPicker.Single` and `Umbraco.DropDown.Single`. A multiple media picker still sends an empty value, as before.

### Internal

* The backoffice client now authenticates with the Umbraco 19 cookie (`authContext.configureClient`) instead of a bearer token, and is built against `@umbraco-cms/backoffice` 19.0.0-rc1.


## [6.0.0] - 2026-06-23

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
