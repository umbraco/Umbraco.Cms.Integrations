using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmHubspot")]
    public class FormsController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        private readonly IAppSettings _appSettings;

        private readonly ITokenService _tokenService;

        private readonly ILogger _logger;

        public const string OAuthClientId = "8b4db9a8-3b78-48bc-85b0-96a4211a1c83";
        private const string OAuthScopes = "oauth forms"; // oauth forms
        private const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364
        private const string OAuthProxyEndpoint = "{0}oauth/v1/token";
        private const string HubspotServiceName = "HubspotForms";

        private const string AccessTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.AccessTokenDbKey";
        private const string RefreshTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.RefreshTokenDbKey";

        private const string HubspotFormsAuthorizationUrl =
            "https://app-eu1.hubspot.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}";
        private const string HubspotFormsApiEndpoint = "https://api.hubapi.com/forms/v2/forms";

        public FormsController(IAppSettings appSettings, ITokenService tokenService, ILogger logger)
        {
            _appSettings = appSettings;

            _tokenService = tokenService;

            _logger = logger;
        }

        public async Task<ResponseDto> GetAll()
        {
            var hubspotApiKey = _appSettings[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotApiKey];

            if (string.IsNullOrEmpty(hubspotApiKey))
            {
                _logger.Info<FormsController>(message: "Cannot access Hubspot - API key is missing");

                return new ResponseDto { IsValid = false };
            }

            var response = await ClientFactory().GetAsync($"{HubspotFormsApiEndpoint}?hapikey=" + hubspotApiKey);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await response.Content.ReadAsStringAsync();
                _logger.Error<FormsController>(
                    $"Failed to fetch forms from Hubspot using API key: {response.ReasonPhrase}");
                return new ResponseDto { IsExpired = true };
            }

            if (response.IsSuccessStatusCode)
            {
                var forms = await response.Content.ReadAsStringAsync();
                var hubspotForms = HubspotForms.FromJson(forms);

                var responseDto = new ResponseDto { IsValid = true };
                foreach (var hubspotForm in hubspotForms)
                {
                    var hubspotFormDto = new HubspotFormDto
                    {
                        Name = hubspotForm.Name,
                        PortalId = hubspotForm.PortalId.ToString(),
                        Id = hubspotForm.Guid,
                        Fields = string.Join(", ", hubspotForm.FormFieldGroups.SelectMany(x => x.Fields).Select(y => y.Label))
                    };
                    responseDto.Forms.Add(hubspotFormDto);
                }

                return responseDto;
            }

            _logger.Error<FormsController>($"Failed to fetch forms from Hubspot using API key: {response.StatusCode} {response.ReasonPhrase}");

            return new ResponseDto();
        }

        public async Task<ResponseDto> GetAllOAuth()
        {
            _tokenService.TryGetParameters(AccessTokenDbKey, out string accessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.Info<FormsController>("Cannot access Hubspot - Access Token is missing.");
                
                return new ResponseDto { IsValid = false };
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(HubspotFormsApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await response.Content.ReadAsStringAsync();

                _logger.Error<FormsController>(
                    $"Failed to fetch forms from Hubspot using OAuth: {response.ReasonPhrase}");
                
                return new ResponseDto { IsExpired = true };
            }

            if (response.IsSuccessStatusCode)
            {
                var forms = await response.Content.ReadAsStringAsync();

                var hubspotForms = HubspotForms.FromJson(forms);

                var responseDto = new ResponseDto { IsValid = true };
                foreach (var hubspotForm in hubspotForms)
                {
                    var hubspotFormDto = new HubspotFormDto
                    {
                        Name = hubspotForm.Name,
                        PortalId = hubspotForm.PortalId.ToString(),
                        Id = hubspotForm.Guid,
                        Fields = string.Join(", ", hubspotForm.FormFieldGroups.SelectMany(x => x.Fields).Select(y => y.Label))
                    };
                    responseDto.Forms.Add(hubspotFormDto);
                }

                return responseDto;
            }

            _logger.Error<FormsController>($"Failed to fetch forms from Hubspot using OAuth: {response.StatusCode} {response.ReasonPhrase}");

            return new ResponseDto();
        }

        [HttpGet]
        public HubspotFormPickerSettings CheckConfiguration()
        {
            if (string.IsNullOrEmpty(_appSettings[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotRegion]))
                return new HubspotFormPickerSettings();

            return
                !string.IsNullOrEmpty(_appSettings[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotApiKey])
                    ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.Api}
                    : !string.IsNullOrEmpty(OAuthClientId)
                       && !string.IsNullOrEmpty(OAuthScopes)
                       && !string.IsNullOrEmpty(OAuthProxyBaseUrl)
                       && !string.IsNullOrEmpty(OAuthProxyEndpoint)
                        ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.OAuth}
                        : new HubspotFormPickerSettings();
        }

        [HttpGet]
        public string GetAuthorizationUrl()
        {
            return string.Format(HubspotFormsAuthorizationUrl, OAuthClientId, OAuthProxyBaseUrl, OAuthScopes);
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto)
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", OAuthClientId },
                { "redirect_uri", OAuthProxyBaseUrl },
                { "code", authRequestDto.Code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(OAuthProxyEndpoint, OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", HubspotServiceName);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                _tokenService.SaveParameters(AccessTokenDbKey, tokenDto.AccessToken);
                _tokenService.SaveParameters(RefreshTokenDbKey, tokenDto.RefreshToken);

                return result;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResult = await response.Content.ReadAsStringAsync();
                var errorDto = JsonConvert.DeserializeObject<ErrorDto>(errorResult);

                return "Error: " + errorDto.Message;
            }

            return "Error: An unexpected error occurred.";
        }

        [HttpPost]
        public async Task<string> RefreshAccessToken()
        {
            _tokenService.TryGetParameters(RefreshTokenDbKey, out string refreshToken);

            var data = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"client_id", OAuthClientId },
                { "refresh_token", refreshToken }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(OAuthProxyEndpoint, OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", HubspotServiceName);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                _tokenService.SaveParameters(AccessTokenDbKey, tokenDto.AccessToken);
                _tokenService.SaveParameters(RefreshTokenDbKey, tokenDto.RefreshToken);

                return result;
            }

            return "error";
        }

        [HttpGet]
        public async Task<ResponseDto> ValidateAccessToken()
        {
            _tokenService.TryGetParameters(AccessTokenDbKey, out string accessToken);

            if (string.IsNullOrEmpty(accessToken)) return new ResponseDto { IsValid = false };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(HubspotFormsApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            return new ResponseDto
            {
                IsValid = response.IsSuccessStatusCode,
                IsExpired = response.StatusCode == HttpStatusCode.Unauthorized
            };
        }

        [HttpPost]
        public void RevokeAccessToken()
        {
            _tokenService.RemoveParameters(AccessTokenDbKey);
            _tokenService.RemoveParameters(RefreshTokenDbKey);
        }
    }
}
