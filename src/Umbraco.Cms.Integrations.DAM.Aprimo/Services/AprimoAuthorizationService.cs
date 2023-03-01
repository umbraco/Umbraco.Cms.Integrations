using Microsoft.Extensions.Options;

using System.Text.Json;

using Umbraco.Cms.Integrations.DAM.Aprimo.Configuration;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public class AprimoAuthorizationService : IAprimoAuthorizationService
    {
        private readonly AprimoSettings _settings;

        private readonly AprimoOAuthSettings _oauthSettings;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly OAuthConfigurationStorage _storage;

        public const string AuthorizationEndpoint = "https://{0}.aprimo.com/login/connect/authorize" +
            "?response_type=code" +
            "&state={1}" +
            "&client_id={2}" +
            "&redirect_uri={3}" +
            "&scope=api offline_access" +
            "&code_challenge={4}" +
            "&code_challenge_method=S256";

        public AprimoAuthorizationService(
            IOptions<AprimoSettings> options, 
            IOptions<AprimoOAuthSettings> oauthOptions,
            IHttpClientFactory httpClientFactory,
            OAuthConfigurationStorage storage)
        {
            _settings = options.Value;

            _oauthSettings = oauthOptions.Value;
            
            _httpClientFactory = httpClientFactory;

            _storage = storage;
        }

        public string GetAuthorizationUrl(OAuthCodeExchange oauthCodeExchange)
        {
            return string.Format(AuthorizationEndpoint,
                _settings.Tenant,
                oauthCodeExchange.State,
                _oauthSettings.ClientId,
                _oauthSettings.RedirectUri,
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
                { "client_id", _oauthSettings.ClientId },
                { "client_secret", _oauthSettings.ClientSecret },
                { "redirect_uri", _oauthSettings.RedirectUri },
                { "code_verifier", configurationEntity.CodeVerifier }
            };

            //var requestMessage = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Post,
            //    RequestUri = new Uri(TokenEndpoint),
            //    Content = new FormUrlEncodedContent(requestData)
            //};
           // requestMessage.Headers.Add("service_name", Service);
            //requestMessage.Headers.Add("tenant", _settings.Tenant);

            var httpClient = _httpClientFactory.CreateClient(Constants.AprimoAuthClient);
            var response = await httpClient.PostAsync("login/connect/token", new FormUrlEncodedContent(requestData));

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
                { "client_id", _oauthSettings.ClientId },
                { "client_secret", _oauthSettings.ClientSecret }
            };

            //var requestMessage = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Post,
            //    RequestUri = new Uri(TokenEndpoint),
            //    Content = new FormUrlEncodedContent(requestData)
            //};
            //requestMessage.Headers.Add("service_name", Service);
            //requestMessage.Headers.Add("tenant", _settings.Tenant);

            var httpClient = _httpClientFactory.CreateClient(Constants.AprimoAuthClient);
            var response = await httpClient.PostAsync("login/connect/token", new FormUrlEncodedContent(requestData));

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
