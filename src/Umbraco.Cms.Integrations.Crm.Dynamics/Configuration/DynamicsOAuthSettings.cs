
using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Configuration
{
    public class DynamicsOAuthSettings
    {
        public DynamicsOAuthSettings() { }

        public DynamicsOAuthSettings(NameValueCollection appSettings)
        {
            ClientId = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsClientIdKey];

            ClientSecret = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsClientSecretKey];

            RedirectUri = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsRedirectUriKey];

            Scopes = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsScopesKey];

            AuthorizationEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsAuthorizationEndpointKey];

            TokenEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsTokenEndpointKey];
        }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
