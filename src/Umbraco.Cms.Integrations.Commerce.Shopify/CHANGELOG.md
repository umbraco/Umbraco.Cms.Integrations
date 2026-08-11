# Changelog - Umbraco.Cms.Integrations.Commerce.Shopify

All notable changes to Umbraco.Cms.Integrations.Commerce.Shopify will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.0.1] - 2026-08-11

### fix

* Store product picker values as JSON. The picker stored no value type, so a second save wrote a type name to the database instead of the selected ids and the page then threw on every request. Includes a migration that moves existing values to the correct column and clears values already corrupted by the bug, which have to be re-picked ([636a07d](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/636a07d))

## [5.0.0] - 2026-08-10

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
