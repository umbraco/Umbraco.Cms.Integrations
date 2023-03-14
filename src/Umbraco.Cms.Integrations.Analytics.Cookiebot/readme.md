# Umbraco.Cms.Integrations.Analytics.Cookiebot

This integration provides an implementation model for [Cookiebot](https://www.cookiebot.com/) banner and declaration.

## Prerequisites

Requires minimum versions of Umbraco CMS: 
- CMS: 10.0.0

## How To Use

### Configuration

The following configuration is required for the Cookiebot scripts to be loaded correctly:

```
"Umbraco": {
  "Cookiebot": {
    "Settings": {
      "Id": "[YOUR_CBID]"
    }
  }
}
```
`CBID` = `Cookiebot Identifier`

### Working with the Umbraco CMS - Cookiebot integration
The package is a reusable [Razor class library](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-6.0&tabs=visual-studio) which will allow editors to load the Cookiebot Banner and Declaration scripts.

Per Cookiebot documentation, the Banner script needs to be inserted as the __very first script__ of the website, by placing it in the _HEAD_ tag using this syntax:

`@await Html.PartialAsync("~/Views/Partials/UmbracoCms.Integrations/Analytics/Cookiebot/Banner.cshtml")`

The Declaration script can be added in whatever page you want, using this syntax:
`@await Html.PartialAsync("~/Views/Partials/UmbracoCms.Integrations/Scripts/Cookiebot/Declaration.cshtml")`

Both scripts "pick up" the `CBID` from the website's settings file
`Configuration["Umbraco:Cookiebot:Settings:Id"]`
and update the details accordingly.


### Custom implementations
This integration showcases how easy it is to work with a script-based provider, just by using partial views and Microsoft's `IConfiguration` interface.

You can use this package as reference for creating your own. To do so, please follow these steps:
- Create a new Razor class library for your integration
- Add partial(s) view(s) where you insert your custom script code
- Inject the `IConfiguration` interface into your view: `@inject Microsoft.Extensions.Configuration.IConfiguration Configuration`
- Use `Configuration[YOUR_SETTINGS_PATH:KEY]` to retrieve the required configuration values
- Add `umbraco-markertplace.json` file with Marketplace details of the package

Once your integration is ready, all that remains to do is to deploy the package to NuGet.
You can use [this](https://learn.microsoft.com/en-us/nuget/what-is-nuget) section of the documentation from Microsoft to get started.



