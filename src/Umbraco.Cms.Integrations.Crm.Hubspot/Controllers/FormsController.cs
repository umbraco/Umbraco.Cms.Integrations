using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

#if NETCOREAPP
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
#else
using System.Web.Http;
using System.Configuration;

using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.Crm.Hubspot.Resources;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;


namespace Umbraco.Cms.Integrations.Crm.Hubspot.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmHubspot")]
    public class FormsController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        // public property to support unit tests
        public HubspotSettings Options;

        private readonly ITokenService _tokenService;

#if NETCOREAPP
        private readonly ILogger<FormsController> _logger;
#else
        private readonly ILogger _logger;
#endif

        public const string OAuthClientId = "1a04f5bf-e99e-48e1-9d62-6c25bf2bdefe";
        private const string OAuthScopes = "oauth forms crm.objects.contacts.read crm.objects.contacts.write";
        private const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364
        private const string OAuthProxyEndpoint = "{0}oauth/v1/token";
        private const string HubspotServiceName = "HubspotForms";

        private const string AccessTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.AccessTokenDbKey";
        private const string RefreshTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.RefreshTokenDbKey";

        private const string HubspotFormsAuthorizationUrl =
            "https://app-eu1.hubspot.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}";
        private const string HubspotFormsApiEndpoint = "https://api.hubapi.com/forms/v2/forms";

#if NETCOREAPP
        public FormsController(IOptions<HubspotSettings> options, ITokenService tokenService, ILogger<FormsController> logger)
        {
            Options = options.Value;

            _tokenService = tokenService;

            _logger = logger;
        }
#else
        public FormsController(ITokenService tokenService, ILogger logger)
        {
            Options = new HubspotSettings(ConfigurationManager.AppSettings);

            _tokenService = tokenService;

            _logger = logger;
        }
#endif

        public async Task<ResponseDto> GetAll()
        {
            var hubspotApiKey = Options.ApiKey;

            if (string.IsNullOrEmpty(hubspotApiKey))
            {
#if NETCOREAPP
                _logger.LogInformation(message: LoggingResources.ApiKeyMissing);
#else
                _logger.Info<FormsController>(message: LoggingResources.ApiKeyMissing);
#endif

                return new ResponseDto { IsValid = false };
            }

            var requestMessage = CreateRequest(hubspotApiKey);

            var response = await ClientFactory().SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
#if NETCOREAPP
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, response.ReasonPhrase));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.ApiFetchFormsFailed, response.ReasonPhrase));
#endif
                
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
                        Region = Options.Region,
                        Fields = string.Join(", ", hubspotForm.FormFieldGroups.SelectMany(x => x.Fields).Select(y => y.Label))
                    };

                    responseDto.Forms.Add(hubspotFormDto);
                }

                return responseDto;
            }

#if NETCOREAPP
            _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, response.StatusCode + " " + response.ReasonPhrase));
#else
            _logger.Error<FormsController>(string.Format(LoggingResources.ApiFetchFormsFailed, response.StatusCode + " " + response.ReasonPhrase));
#endif

            return new ResponseDto();
        }

        public async Task<ResponseDto> GetAllOAuth()
        {
            _tokenService.TryGetParameters(AccessTokenDbKey, out string accessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
#if NETCOREAPP
                _logger.LogInformation(LoggingResources.AccessTokenMissing);
#else
                _logger.Info<FormsController>(LoggingResources.AccessTokenMissing);
#endif

                return new ResponseDto { IsValid = false };
            }

            var requestMessage = CreateRequest(accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
#if NETCOREAPP
                _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, response.ReasonPhrase));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.OAuthFetchFormsFailed, response.ReasonPhrase));
#endif     

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
                        Region = Options.Region,
                        Fields = string.Join(", ", hubspotForm.FormFieldGroups.SelectMany(x => x.Fields).Select(y => y.Label))
                    };
                    responseDto.Forms.Add(hubspotFormDto);
                }

                return responseDto;
            }

#if NETCOREAPP
            _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, response.StatusCode + " " + response.ReasonPhrase));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.OAuthFetchFormsFailed, response.StatusCode + " " + response.ReasonPhrase));
#endif   

            return new ResponseDto();
        }

        private static HttpRequestMessage CreateRequest(string accessToken)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(HubspotFormsApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }

        [HttpGet]
        public HubspotFormPickerSettings CheckConfiguration()
        {
            return
                !string.IsNullOrEmpty(Options.ApiKey)
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
