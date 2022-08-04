using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

#if NETCOREAPP
using Microsoft.Extensions.Options;
#else
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class DynamicsService
    {
        private readonly DynamicsSettings _settings;

        private readonly DynamicsConfigurationService _dynamicsConfigurationService;

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

#if NETCOREAPP
        public DynamicsService(IOptions<DynamicsSettings> options, DynamicsConfigurationService dynamicsConfigurationService)
        {
            _settings = options.Value;

            _dynamicsConfigurationService = dynamicsConfigurationService;
        }
#else
        public DynamicsService(DynamicsConfigurationService dynamicsConfigurationService)
        {
            _settings = new DynamicsSettings(ConfigurationManager.AppSettings);

            _dynamicsConfigurationService = dynamicsConfigurationService;
        }
#endif

        public async Task<IdentityDto> GetIdentity(string accessToken)
        {
            var userId = await GetUserId(accessToken);

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_settings.HostUrl}{_settings.ApiPath}systemusers")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return new IdentityDto { IsAuthorized = false };

            var result = await response.Content.ReadAsStringAsync();

            var systemUsers = JsonConvert.DeserializeObject<ResponseDto<IdentityDto>>(result);

            return systemUsers.Value.FirstOrDefault(p => p.UserId == userId.ToString());
        }

        public async Task<string> GetEmbedCode(string formId)
        {
            var oauthConfiguration = _dynamicsConfigurationService.GetOAuthConfiguration();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_settings.HostUrl}{_settings.ApiPath}msdyncrm_formpages?$select=msdyncrm_javascriptcode,_msdyncrm_marketingformid_value" +
                    $"&$filter=_msdyncrm_marketingformid_value eq {formId}")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", oauthConfiguration.AccessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode) return String.Empty;

            var result = await response.Content.ReadAsStringAsync();

            var embedCode = JsonConvert.DeserializeObject<ResponseDto<FormDto>>(result);

            return embedCode.Value.First().EmbedCode;
        }

        private async Task<string> GetUserId(string accessToken)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_settings.HostUrl}{_settings.ApiPath}WhoAmI")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            if(!response.IsSuccessStatusCode) return String.Empty;

            var result = await response.Content.ReadAsStringAsync();

            return JObject.Parse(result)["UserId"].ToString();
        }
    }
}
