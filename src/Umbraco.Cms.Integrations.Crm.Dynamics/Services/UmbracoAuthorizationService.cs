using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class UmbracoAuthorizationService : BaseAuthorizationService, IDynamicsAuthorizationService
    {
        private readonly DynamicsSettings _settings;

        public const string ClientId = "813c5a65-cfd6-48d6-8928-dffe02aaf61a";

        public const string Service = "Dynamics";

        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364;

        public const string OAuthProxyRedirectUrl = OAuthProxyBaseUrl + "oauth/dynamics/";

        public const string OAuthProxyTokenUrl = OAuthProxyBaseUrl + "oauth/v1/token";

        protected const string OAuthScopes = "{0}.default";

        public UmbracoAuthorizationService(
            IOptions<DynamicsSettings> options, 
            IDynamicsService dynamicsService, 
            IDynamicsConfigurationStorage dynamicsConfigurationStorage) 
            : base(dynamicsService, dynamicsConfigurationStorage)
        {
            _settings = options.Value;
        }

        public string GetAuthorizationUrl()
        {
            var scopes = string.Format(OAuthScopes, _settings.HostUrl);
            return string.Format(DynamicsAuthorizationUrl, ClientId, OAuthProxyRedirectUrl, scopes);
        }

        public string GetAccessToken(string code) => 
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", ClientId },
                { "redirect_uri", OAuthProxyRedirectUrl },
                { "code", code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthProxyTokenUrl),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", Service);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonSerializer.Deserialize<TokenDto>(result);

                var identity = await DynamicsService.GetIdentity(tokenDto.AccessToken);

                if (identity.IsAuthorized)
                    DynamicsConfigurationStorage.AddOrUpdateOAuthConfiguration(tokenDto.AccessToken, identity.UserId, identity.FullName);
                else
                    return "Error: " + identity.Error.Message;

                return result;
            }

            var errorResult = await response.Content.ReadAsStringAsync();
            var errorDto = JsonSerializer.Deserialize<ErrorDto>(errorResult);

            return "Error: " + errorDto.ErrorDescription;
        }
    }
}
