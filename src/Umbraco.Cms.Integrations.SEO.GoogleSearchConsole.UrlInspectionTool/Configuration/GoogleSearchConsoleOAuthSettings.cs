using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration
{
    public class GoogleSearchConsoleOAuthSettings
    {
        public GoogleSearchConsoleOAuthSettings() { }

        public GoogleSearchConsoleOAuthSettings(NameValueCollection appSettings)
        {
            ClientId = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoGoogleSearchConsoleClientIdKey];

            ClientSecret = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoGoogleSearchConsoleClientSecretKey];

            RedirectUri = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoGoogleSearchConsoleRedirectUriKey];

            Scopes = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoGoogleSearchConsoleScopesKey];

            TokenEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoGoogleSearchConsoleTokenEndpointKey];
        }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
