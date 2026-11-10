# Changelog - Umbraco.Cms.Integrations.Search.Algolia

All notable changes to Umbraco.Cms.Integrations.Search.Algolia will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [8.0.0] - Unreleased

### Breaking

* Requires Umbraco CMS 19 (built against 19.0.0-beta1) and .NET 11. The package targets `net11.0` and depends on `Umbraco.Cms.*` `[19.0.0-beta1, 20.0.0)`. Use the 7.x line for Umbraco 18.
* `IAlgoliaIndexValueConverter.ParseIndexValues(IProperty, IndexValue)` and `UmbracoBlockValueConverterBase.ParseIndexValues` are now `ParseIndexValues(IProperty property, string? culture)`. The CMS type they took (`IndexValue`) is removed in Umbraco 19, so custom converters must be updated.
* Editors without an Algolia converter: core editors get the package's own copy of the Umbraco 18 index output; any other (third-party or custom) editor is indexed as its raw stored value, because its own `PropertyIndexValueFactory` no longer exists in Umbraco 19. Sites with such editors should register an `IAlgoliaIndexValueConverter`.
* A Single Block property is indexed by the new `UmbracoSingleBlockConverter` as a list of strings, like Block List and Block Grid, instead of one concatenated summary. A Single Block nested in a Block List or Grid is still skipped.

### feat

* Build index values in the package, since Umbraco 19 removes `IDataEditor.PropertyIndexValueFactory` and `IndexValue`.

### fix

* Format date and time values with the invariant culture. They previously followed the server culture (`08.30.00` under Danish).

### Internal

* The backoffice client now authenticates with the Umbraco 19 cookie (`authContext.configureClient`) instead of a bearer token, and is built against `@umbraco-cms/backoffice` 19.0.0-beta1.


## [7.0.2] - 2026-08-11

### fix

* Skip records Algolia rejects instead of abandoning the index build. An index build now pushes in chunks and retries a rejected chunk one record at a time, so a single oversized record no longer leaves the rest of the site unindexed ([bd806b5](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/bd806b5))
* Remove descendants from the Algolia index when a branch is deleted. Deleting a parent previously left every descendant behind in the index ([6e6fdab](https://github.com/umbraco/Umbraco.Cms.Integrations/commit/6e6fdab))

## [7.0.1] - 2026-07-16

### Internal

* Baseline entry, recorded when this repo moved to per-package `version.json` versioning.
  Changes before this version are in the git history and the GitHub releases.
