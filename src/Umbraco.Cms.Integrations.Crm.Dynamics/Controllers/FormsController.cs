#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
#else
using System.Configuration;
using System.Web.Http;

using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using System.Linq;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmDynamics")]
    public class FormsController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        private readonly DynamicsSettings _settings;

        private readonly IAuthorizationService _authorizationService;

        private readonly DynamicsService _dynamicsService;

        private readonly DynamicsConfigurationService _dynamicsConfigurationService;

#if NETCOREAPP
        public FormsController(IOptions<DynamicsSettings> options, IAuthorizationService authorizationService, 
            DynamicsService dynamicsService, 
            DynamicsConfigurationService dynamicsConfigurationService)
#else
        public FormsController(IAuthorizationService authorizationService,
            DynamicsService dynamicsService,
            DynamicsConfigurationService dynamicsConfigurationService)
#endif
        {
            _authorizationService = authorizationService;

            _dynamicsService = dynamicsService;

            _dynamicsConfigurationService = dynamicsConfigurationService;

#if NETCOREAPP
            _settings = options.Value;
#else
            _settings = new DynamicsSettings(ConfigurationManager.AppSettings);
#endif
        }

        [HttpGet]
        public string GetAuthorizationUrl() => _authorizationService.GetAuthorizationUrl();

        [HttpGet]
        public async Task<OAuthConfigurationDto> CheckOAuthConfiguration()
        {
            var oauthConfiguration = _dynamicsConfigurationService.GetOAuthConfiguration();

            if (oauthConfiguration == null) return new OAuthConfigurationDto { Message = string.Empty };

            var identity = await _dynamicsService.GetIdentity(oauthConfiguration.AccessToken);

            if (!identity.IsAuthorized) return new OAuthConfigurationDto { Message = identity.Error?.Message };

            oauthConfiguration.IsAuthorized = true;

            return oauthConfiguration;
        } 

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto)
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", AuthorizationService.ClientId },
                { "redirect_uri", AuthorizationService.RedirectUri },
                { "code", authRequestDto.Code } 
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(AuthorizationService.OAuthProxyTokenEndpoint, AuthorizationService.OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", AuthorizationService.Service);

            var response = await ClientFactory().SendAsync(requestMessage);
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                var identity = await _dynamicsService.GetIdentity(tokenDto.AccessToken);

                if (identity.IsAuthorized)
                    _dynamicsConfigurationService.AddorUpdateOAuthConfiguration(tokenDto.AccessToken, identity.UserId, identity.FullName);
                else
                    return "Error: " + identity.Error.Message;

                return result;

            } else
            {
                var errorResult = await response.Content.ReadAsStringAsync();
                var errorDto = JsonConvert.DeserializeObject<ErrorDto>(errorResult);

                return "Error: " + errorDto.ErrorDescription;
            }
        }

        [HttpGet]
        public async Task<ResponseDto<FormDto>> GetForms()
        {
            var oauthConfiguration = _dynamicsConfigurationService.GetOAuthConfiguration();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_settings.HostUrl}{_settings.ApiPath}msdyncrm_marketingforms")
            };
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", oauthConfiguration.AccessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ResponseDto<FormDto>>(result);
        }

        [HttpGet]
        public async Task<string> GetEmbedCode(string formId) => await _dynamicsService.GetEmbedCode(formId);

        [HttpGet]
        public string GetSystemUserFullName() => _dynamicsConfigurationService.GetSystemUserFullName();

        [HttpDelete]
        public string RevokeAccessToken() => _dynamicsConfigurationService.Delete();
    }
}
