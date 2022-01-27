﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Lucene.Net.Analysis;
using Newtonsoft.Json;
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
        internal static Func<HttpClient> ClientFactory = () => s_client;

        private readonly IHubspotService _hubspotService;

        private readonly ITokenService _tokenService;

        private const string AccessTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.AccessTokenDbKey";

        private const string RefreshTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.RefreshTokenDbKey";

        private const string HubspotFormsApiEndpoint = "https://api.hubapi.com/forms/v2/forms";

        public FormsController(IHubspotService hubspotService, ITokenService tokenService)
        {
            _hubspotService = hubspotService;

            _tokenService = tokenService;
        }

        public async Task<ResponseDto> GetAll()
        {
            var hubspotApiKey = ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.ApiKey"];

            if (string.IsNullOrEmpty(hubspotApiKey))
            {
                Logger.Error(typeof(FormsController), $"{nameof(FormsController)}.{nameof(GetAll)} - API Key is missing");
                return new ResponseDto { IsValid = false };
            }

            var response = await ClientFactory().GetAsync($"{HubspotFormsApiEndpoint}?hapikey=" + hubspotApiKey);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await response.Content.ReadAsStringAsync();
                Logger.Error(typeof(FormsController), $"{nameof(FormsController)}.{nameof(GetAll)} - API Key rejected with message: {result}");
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

            return new ResponseDto();
        }

        public async Task<ResponseDto> GetAllOAuth()
        {
            _tokenService.TryGetParameters(AccessTokenDbKey, out string accessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                Logger.Error(typeof(FormsController), $"{nameof(FormsController)}.{nameof(GetAllOAuth)} - Access Token is missing.");
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
                Logger.Error(typeof(FormsController), $"{nameof(FormsController)}.{nameof(GetAllOAuth)} - Access Token denied with message: {result}");
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

            return new ResponseDto();
        }

        [HttpGet]
        public bool CheckApiConfiguration()
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.ApiKey"]);
        }

        [HttpGet]
        public bool CheckOAuthConfiguration()
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthClientId"])
                && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthScopes"])
                && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthRedirectUrl"])
                && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.OAuthProxyUrl"]);
        }

        [HttpGet]
        public string GetAuthorizationUrl()
        {
            return _hubspotService.GetAuthorizationUrl();
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto)
        {
            var data = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"client_id", ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthClientId"]},
                {
                    "redirect_uri",
                    ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthRedirectUrl"]
                },
                { "code", authRequestDto.Code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.OAuthProxyUrl"]}"),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", "Hubspot");

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

        [HttpPost]
        public async Task<string> RefreshAccessToken()
        {
            _tokenService.TryGetParameters(RefreshTokenDbKey, out string refreshToken);

            var data = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"client_id", ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthClientId"]},
                { "refresh_token", refreshToken }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.OAuthProxyUrl"]}"),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", "Hubspot");

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
    }
}