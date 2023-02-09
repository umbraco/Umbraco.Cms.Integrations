using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Umbraco.Cms.Integrations.DAM.Aprimo.Configuration;
using Umbraco.Cms.Integrations.DAM.Aprimo.Extensions;
using Umbraco.Cms.Integrations.DAM.Aprimo.Migrations;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Controllers
{
    [PluginController("UmbracoCmsIntegrationsDamAprimo")]
    public class AssetsController : UmbracoAuthorizedApiController
    {
        private readonly AprimoSettings _settings;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IAprimoAuthorizationService _aprimoAuthorizationService;

        private readonly OAuthConfigurationStorage _oauthConfigurationStorage;

        public AssetsController(IOptions<AprimoSettings> options, 
            IHttpClientFactory httpClientFactory, 
            IAprimoAuthorizationService aprimoAuthorizationService,
            OAuthConfigurationStorage oauthConfigurationStorage)
        {
            _settings = options.Value;

            _httpClientFactory = httpClientFactory;

            _aprimoAuthorizationService= aprimoAuthorizationService;

            _oauthConfigurationStorage = oauthConfigurationStorage;
        }

        [HttpGet]
        public string GetAuthorizationUrl()
        {
            var oauthCodeExchange = OAuthHelper.GenerateKeys();

            var configurationEntity = new AprimoOAuthConfiguration
            {
                AccessToken = string.Empty,
                RefreshToken= string.Empty,
                CodeVerifier = oauthCodeExchange.CodeVerifier,
                CodeChallenge = oauthCodeExchange.CodeChallenge
            };
            _oauthConfigurationStorage.AddOrUpdate(configurationEntity);

            return _aprimoAuthorizationService.GetAuthorizationUrl(oauthCodeExchange);
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequest request)
        {
            var configurationEntity = _oauthConfigurationStorage.Get();
            if (configurationEntity == null) return "Error: Invalid code challenge.";

            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", request.Code },
                { "client_id", _settings.ClientId },
                { "redirect_uri", _settings.RedirectUri },
                { "code_verifier", configurationEntity.CodeVerifier }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(AprimoAuthorizationService.TokenEndpoint),
                Content = new FormUrlEncodedContent(requestData)
            };
            requestMessage.Headers.Add("service_name",  AprimoAuthorizationService.Service);
            requestMessage.Headers.Add("tenant", _settings.Tenant);

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<OAuthResponse>(content);
                if (data == null) return "Error: Failed to retrieve the access token.";

                configurationEntity.AccessToken = data.AccessToken;
                configurationEntity.RefreshToken= data.RefreshToken;

                _oauthConfigurationStorage.AddOrUpdate(configurationEntity);

                return string.Empty;
            }

            _oauthConfigurationStorage.Delete(configurationEntity.Id);

            return "Error";
        }
    }
}
