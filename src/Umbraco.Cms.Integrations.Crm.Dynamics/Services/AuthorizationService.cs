using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;

#if NETCOREAPP
using Microsoft.Extensions.Options;
#else
using System.Configuration;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly DynamicsSettings _settings;

        public const string ClientId = "813c5a65-cfd6-48d6-8928-dffe02aaf61a";

        public const string RedirectUri = OAuthProxyBaseUrl;

        public const string Service = "Dynamics";

        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364;

        public const string OAuthProxyTokenEndpoint = "{0}oauth/v1/token";

        protected const string OAuthScopes = "{0}.default";

        protected const string DynamicsAuthorizationUrl =
            "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={0}&response_type=code&redirect_uri={1}&response_mode=query&scope={2}";

#if NETCOREAPP
        public AuthorizationService(IOptions<DynamicsSettings> options)
        {
            _settings = options.Value;
        }
#else
        public AuthorizationService()
        {
            _settings = new DynamicsSettings(ConfigurationManager.AppSettings);
        }
#endif

        public string GetAuthorizationUrl()
        {
            var scopes = string.Format(OAuthScopes, _settings.HostUrl);
            return string.Format(DynamicsAuthorizationUrl, ClientId, OAuthProxyBaseUrl, scopes);
        }
            
    }
}
