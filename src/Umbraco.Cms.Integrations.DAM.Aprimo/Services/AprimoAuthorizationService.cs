using Microsoft.Extensions.Options;

using Umbraco.Cms.Integrations.DAM.Aprimo.Configuration;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public class AprimoAuthorizationService : IAprimoAuthorizationService
    {
        private readonly AprimoSettings _settings;

        public const string Service = "Aprimo";

        public const string AuthorizationEndpoint = "https://{0}.aprimo.com/login/connect/authorize" +
            "?response_type=code" +
            "&state={1}" +
            "&client_id={2}" +
            "&redirect_uri={3}" +
            "&scope=api offline_access" +
            "&code_challenge={4}" +
            "&code_challenge_method=S256";

        public const string TokenEndpoint = "https://localhost:44364/oauth/v1/token";

        //public const string OAuthProxyBaseUrl = "https://localhost:44364/"; //"https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364/;

        //public const string OAuthProxyTokenEndpoint = "{0}oauth/v1/token";

        public AprimoAuthorizationService(IOptions<AprimoSettings> options)
        {
            _settings= options.Value;
        }

        public string GetAuthorizationUrl(OAuthCodeExchange oauthCodeExchange)
        {
            return string.Format(AuthorizationEndpoint,
                _settings.Tenant,
                oauthCodeExchange.State,
                _settings.ClientId,
                _settings.RedirectUri,
                oauthCodeExchange.CodeChallenge);

        }
    }
}
