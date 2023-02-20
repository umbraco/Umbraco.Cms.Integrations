using Microsoft.Extensions.Options;
using System.Text.Json;
using Umbraco.Cms.Integrations.DAM.Aprimo.Configuration;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public class AprimoAuthorizationService : IAprimoAuthorizationService
    {
        private readonly AprimoSettings _settings;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly OAuthConfigurationStorage _storage;

        public const string Service = "Aprimo";

        public const string AuthorizationEndpoint = "https://{0}.aprimo.com/login/connect/authorize" +
            "?response_type=code" +
            "&state={1}" +
            "&client_id={2}" +
            "&redirect_uri={3}" +
            "&scope=api offline_access" +
            "&code_challenge={4}" +
            "&code_challenge_method=S256";

        public const string TokenEndpoint = "https://localhost:44364/oauth/v1/token"; //"https://hubspot-forms-auth.umbraco.com/"

        public AprimoAuthorizationService(
            IOptions<AprimoSettings> options, 
            IHttpClientFactory httpClientFactory,
            OAuthConfigurationStorage storage)
        {
            _settings = options.Value;
            
            _httpClientFactory = httpClientFactory;

            _storage = storage;
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

        public async Task<string> GetAccessToken(string code)
        {
            var configurationEntity = _storage.Get();
            if (configurationEntity == null) return "Error: " + Constants.ErrorResources.InvalidCodeChallenge;

            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "client_id", _settings.ClientId },
                { "redirect_uri", _settings.RedirectUri },
                { "code_verifier", configurationEntity.CodeVerifier }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(TokenEndpoint),
                Content = new FormUrlEncodedContent(requestData)
            };
            requestMessage.Headers.Add("service_name", Service);
            requestMessage.Headers.Add("tenant", _settings.Tenant);

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<OAuthResponse>(content);
                if (data == null) return "Error: " + Constants.ErrorResources.RetrieveAccessToken;

                configurationEntity.AccessToken = data.AccessToken;
                configurationEntity.RefreshToken = data.RefreshToken;

                _storage.AddOrUpdate(configurationEntity);

                return string.Empty;
            }

            _storage.Delete();

            return "Error: " + content;
        }

        public async Task<string> RefreshAccessToken()
        {
            var configurationEntity = _storage.Get();
            if (configurationEntity == null) return Constants.ErrorResources.MissingRefreshToken;

            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", configurationEntity.RefreshToken },
                { "client_id", _settings.ClientId }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(TokenEndpoint),
                Content = new FormUrlEncodedContent(requestData)
            };
            requestMessage.Headers.Add("service_name", Service);
            requestMessage.Headers.Add("tenant", _settings.Tenant);

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<OAuthResponse>(content);
                if (data == null) return Constants.ErrorResources.InvalidAuthorizationResponse;

                configurationEntity.AccessToken = data.AccessToken;
                configurationEntity.RefreshToken = data.RefreshToken;

                _storage.AddOrUpdate(configurationEntity);
            }

            return "Error: " + content;
        }
    }
}
