
using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Configuration
{
    public class SemrushOAuthSettings
    {
        public SemrushOAuthSettings()
        { }

        public SemrushOAuthSettings(NameValueCollection appSettings)
        {
            Ref = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushRefKey];

            ClientId = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushClientIdKey];

            ClientSecret = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushClientSecretKey];

            RedirectUri = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushRedirectUriKey];

            Scopes = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushScopesKey];

            AuthorizationEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushAuthorizationEndpointKey];

            TokenEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushTokenEndpointKey];
        }

        public string Ref { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
