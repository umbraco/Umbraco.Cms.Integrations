# Umbraco.Cms.Integrations.SEO.Semrush

This integration provides a keywords search tool powered by [Semrush](https://www.semrush.com/).

## Prerequisites

Requires minimum versions of Umbraco CMS: 
- CMS V8: 8.5.4
- CMS V9: 9.0.1

An account with Semrush.

## How To Use

Once the package is installed either from [NuGet](https://www.nuget.org/packages/Umbraco.Cms.Integrations.Seo.Semrush) or via the zip file available at our.umbraco.com _(link TBC when published)_, the integration is made available in the Umbraco back-office as a content app.

The interface is available for all types of content items as well as for all user groups.  Either using an existing content field as a starting point, or by entering a custom phrase, a keyword search tool can be made for different metrics across various markets.

Adminstrators are provided with additional features for managing the connectivity with their organisation's account with Semrush.

For more detail on the integration, it's purpose and how it was built, please see the accompanying blog post _(link TBC when published)_.

## Authorization Workflow
Starting with version 1.2.0 of the integration, the OAuth flow can be configured to use different Authorization Servers without requests routed through the `OAuth Proxy for Umbraco Integrations`.

### Configuration
To use the new setup, the following configuration is used:
```
<appSettings>
...
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.BaseUrl" value="https://oauth.semrush.com/" />
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.UseUmbracoAuthorization" value="true" />
    <add key="Umbraco.Cms.Integrations.SEO.Semrush.Ref" value="[REF]" />
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.ClientId" value="[YOUR_CLIENT_ID]" />
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.ClientSecret" value="[YOUR_CLIENT_SECRET]" />
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.RedirectUri" value="https://[YOUR_WEBSITE_BASE_URL]/umbraco/api/semrushauthorization/oauth" />
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.Scopes" value="[SCOPES]" />
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.AuthorizationEndpoint" value="[AUTHORIZATION_ENDPOINT]" />
  <add key="Umbraco.Cms.Integrations.SEO.Semrush.TokenEndpoint" value="[TOKEN_ENDPOINT]" />
...
</appSettings>
```

or, 

```
 "Umbraco": {
    "CMS": {
      "Integrations": {
        "SEO": {
          "Semrush": {
            "Settings": {
              "BaseUrl": "https://oauth.semrush.com/",
              "UseUmbracoAuthorization": true
            },
            "OAuthSettings": {
              "Ref": "[REF]",
              "ClientId": "[YOUR_CLIENT_ID]",
              "ClientSecret": "[YOUR_CLIENT_SECRET]",
              "RedirectUri": "https://[YOUR_WEBSITE_BASE_URL]/umbraco/api/semrushauthorization/oauth",
              "AuthorizationEndpoint": "[AUTHORIZATION_ENDPOINT]",
              "TokenEndpoint": "[TOKEN_ENDPOINT]",
              "Scopes": "[SCOPES]"
            }
          },
        }
      }
    }
  }
```

### Configuration Breakdown
- Ref - reference number received from Semrush
- ClientId - client id of your own app
- ClientSecret - client secret of your own app
- RedirectUri - endpoint for Authorization controller that will receive the authorization code from the auth server
- AuthorizationEndpoint - provider URL for handling authorization
- TokenEndpoint - provider URL for retrieving access tokens

### Implementation
The `UseUmbracoAuthorization` flag will toggle between the default Umbraco authorization service and the custom one that will use your private configuration.

`UmbracoAuthorizationService` provides the same implementation as on the previous versions of the package, while the `AuthorizationService` builds a custom authorization flow based on the provided settings.

Both implement `ISemrushAuthorizationService` endpoint and provide the required endpoints for building the authorization URL and retrieving the acces token.

The `AuthorizationImplementationFactory` is used to load the proper injected authorization service.

`SemrushAuthorizationController` with its `OAuth` action will handle the response from the Authorization Server and send the proper message to the client using the `window.opener` interface.





