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
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;


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
            var user = await GetUser(accessToken);

            if (!user.IsAuthorized) return user;

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_settings.HostUrl}{_settings.ApiPath}systemusers")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return new IdentityDto { IsAuthorized = false };

            var result = await response.Content.ReadAsStringAsync();

            var systemUser = JsonConvert.DeserializeObject<ResponseDto<IdentityDto>>(result).Value.FirstOrDefault(p => p.UserId == user.UserId.ToString());
            systemUser.IsAuthorized = true;

            return systemUser;
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

            var embedCode = JsonConvert.DeserializeObject<ResponseDto<OutboundFormDto>>(result);

            return embedCode.Value.FirstOrDefault() != null ? embedCode.Value.First().EmbedCode : string.Empty;
        }

        private async Task<IdentityDto> GetUser(string accessToken)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_settings.HostUrl}{_settings.ApiPath}WhoAmI")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return !string.IsNullOrEmpty(result)
                    ? JsonConvert.DeserializeObject<IdentityDto>(result)
                    : new IdentityDto { IsAuthorized = false };
            }

            return new IdentityDto
            {
                IsAuthorized = true,
                UserId = JObject.Parse(result)["UserId"].ToString()
            };
        }

        public async Task<IEnumerable<FormDto>> GetForms(DynamicsModule module)
        {
            var list = new List<FormDto>();

            var oauthConfiguration = _dynamicsConfigurationService.GetOAuthConfiguration();

            if (module.HasFlag(DynamicsModule.Outbound))
            {
                var forms = await Get<OutboundFormDto>(oauthConfiguration.AccessToken, Constants.Modules.OutboundPath);

                if (forms != null && forms.Value != null && forms.Value.Any())
                {
                    list.AddRange(forms.Value.Select(p => new FormDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Module = DynamicsModule.Outbound
                    }));
                }
            }

            if (module.HasFlag(DynamicsModule.RealTime))
            {
                var forms = await Get<RealTimeFormDto>(oauthConfiguration.AccessToken, Constants.Modules.RealTimePath);
                if (forms != null && forms.Value != null && forms.Value.Any())
                {
                    list.AddRange(forms.Value.Select(p => new FormDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Module = DynamicsModule.RealTime
                    }));
                }
            }

            return list;
        }

        public async Task<FormDto> GetRealTimeForm(string id)
        {
            var oauthConfiguration = _dynamicsConfigurationService.GetOAuthConfiguration();

            var forms = await Get<RealTimeFormDto>(oauthConfiguration.AccessToken, Constants.Modules.RealTimePath);

            if (forms == null || !forms.Value.Any(p => p.Id == id)) return null;

            var form = forms.Value.First(p => p.Id == id);
            return new FormDto
            {
                RawHtml = form.FormHtml,
                StandaloneHtml = form.StandaloneHtml
            };
        }

        private async Task<ResponseDto<T>> Get<T>(string accessToken, string modulePath)
            where T: class
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_settings.HostUrl}{_settings.ApiPath}{modulePath}")
            };
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ResponseDto<T>>(result);
        }
    }
}
