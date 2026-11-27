# Changelog - Umbraco.Cms.Integrations.Crm.Hubspot

All notable changes to Umbraco.Cms.Integrations.Crm.Hubspot will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [9.0.4] - Unreleased

### Internal

* Development version bump after the 9.0.3 release.
* The NuGet package now declares its MIT license.

## [9.0.3] - 2026-11-26

### fix

* **hubspot:** The form picker refreshes an expired OAuth access token and retries, instead of showing "Unable to connect to HubSpot" until the data type settings are opened ([97d4726](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/97d4726))
* **hubspot:** Connecting with OAuth exchanges the authorization code once, so clicking Connect more than once no longer fails with "missing or unknown auth code" ([cd227b2](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/cd227b2))

## [9.0.2] - 2026-08-11

### fix

* Show the correct configuration description in the form picker's authorization panel, which reported "No API or OAuth configuration could be found" even on a correctly configured site ([0f031e3](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/0f031e3))

## [9.0.1] - 2026-07-16

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
