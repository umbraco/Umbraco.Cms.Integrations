# Changelog - Umbraco.Cms.Integrations.Commerce.Shopify

All notable changes to Umbraco.Cms.Integrations.Commerce.Shopify will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [7.0.0] - Unreleased

### Breaking

* Requires Umbraco CMS 19 (built against 19.0.0-beta1) and .NET 11. The package targets `net11.0` and depends on `Umbraco.Cms.*` `[19.0.0-beta1, 20.0.0)`. Use the 6.x line for Umbraco 18.

### Internal

* The backoffice client now authenticates with the Umbraco 19 cookie (`authContext.configureClient`) instead of a bearer token, and is built against `@umbraco-cms/backoffice` 19.0.0-beta1.
* The Shopify product picker setting defaults to `PropertyCacheLevel.None` instead of the obsolete `Snapshot`; product data is still fetched live, so behaviour is unchanged.

## [6.0.1] - 2026-08-11

### fix

* Store product picker values as JSON. The picker stored no value type, so a second save wrote a type name to the database instead of the selected ids and the page then threw on every request. Includes a migration that moves existing values to the correct column and clears values already corrupted by the bug, which have to be re-picked ([f05a60b](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/f05a60b))

## [6.0.0] - 2026-06-23

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
