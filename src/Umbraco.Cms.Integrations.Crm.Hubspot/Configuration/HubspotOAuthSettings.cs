using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Configuration
{
    public class HubspotOAuthSettings
    {
        public HubspotOAuthSettings() { }

        public HubspotOAuthSettings(NameValueCollection appSettings)
        {
            ClientId = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotClientIdKey];

            ClientSecret = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotClientSecretKey];

            RedirectUri = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotRedirectUriKey];

            Scopes = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotScopesKey];

            AuthorizationEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotAuthorizationEndpointKey];

            TokenEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotTokenEndpointKey];
        }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
