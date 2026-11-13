# Changelog - Umbraco.Cms.Integrations.Crm.ActiveCampaign

All notable changes to Umbraco.Cms.Integrations.Crm.ActiveCampaign will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [8.0.0-rc1] - Unreleased

### Breaking

* Requires Umbraco CMS 19 (built against 19.0.0-rc1) and .NET 11. The package targets `net11.0` and depends on `Umbraco.Cms.*` `[19.0.0-rc1, 20.0.0)`. Use the 7.x line for Umbraco 18.

### fix

* The forms endpoints return 502 Bad Gateway with ProblemDetails ("ActiveCampaign authorization failed.") when ActiveCampaign rejects the API key, instead of passing on its 401/403. On Umbraco 19 a passed-on 401 opened the backoffice login dialog and a 403 failed silently.

### Internal

* The backoffice client now authenticates with the Umbraco 19 cookie (`authContext.configureClient`) instead of a bearer token, and is built against `@umbraco-cms/backoffice` 19.0.0-rc1.
* Form picker value converters use `PropertyCacheLevel.Element` instead of the obsolete `Snapshot`; the converted value depends only on the stored value, so behaviour is unchanged.

## [7.0.1] - 2026-07-16

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
