using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class DynamicsService : IDynamicsService
    {
        private readonly DynamicsSettings _settings;

        private readonly IDynamicsConfigurationStorage _dynamicsConfigurationStorage;

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        private const string RealTimeFormCacheKeyPrefix = "Umbraco.Cms.Integrations.Crm.Dynamics.RealTimeForm_";

        private readonly IAppPolicyCache _runtimeCache;

        private readonly ILogger<DynamicsService> _logger;

        [Obsolete("Use the constructor that takes AppCaches and ILogger<DynamicsService>. Scheduled for removal in the next major version.")]
        public DynamicsService(IOptions<DynamicsSettings> options, IDynamicsConfigurationStorage dynamicsConfigurationStorage)
            : this(
                options,
                dynamicsConfigurationStorage,
                StaticServiceProvider.Instance.GetRequiredService<AppCaches>(),
                StaticServiceProvider.Instance.GetRequiredService<ILogger<DynamicsService>>())
        {
        }

        public DynamicsService(
            IOptions<DynamicsSettings> options,
            IDynamicsConfigurationStorage dynamicsConfigurationStorage,
            AppCaches appCaches,
            ILogger<DynamicsService> logger)
        {
            _settings = options.Value;

            _dynamicsConfigurationStorage = dynamicsConfigurationStorage;

            _runtimeCache = appCaches.RuntimeCache;

            _logger = logger;
        }

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

            var systemUser = JsonSerializer.Deserialize<ResponseDto<IdentityDto>>(result).Value.FirstOrDefault(p => p.UserId == user.UserId.ToString());
            systemUser.IsAuthorized = true;

            return systemUser;
        }

        public async Task<string> GetEmbedCode(string formId)
        {
            var oauthConfiguration = _dynamicsConfigurationStorage.GetOAuthConfiguration();

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

            var embedCode = JsonSerializer.Deserialize<ResponseDto<OutboundFormDto>>(result);

            return embedCode.Value.FirstOrDefault() != null ? embedCode.Value.First().EmbedCode : string.Empty;
        }

        public async Task<IEnumerable<FormDto>> GetForms(DynamicsModule module)
        {
            var list = new List<FormDto>();

            var oauthConfiguration = _dynamicsConfigurationStorage.GetOAuthConfiguration();

            if (module.HasFlag(DynamicsModule.Outbound))
            {
                var forms = await Get<OutboundFormDto>(oauthConfiguration.AccessToken, Constants.Modules.OutboundPath);
                if(forms != null)
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
                if (forms != null)
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

        /// <summary>
        /// Gets a real-time form for rendering. The form is fetched with the stored access token, which
        /// can expire. When the fetch fails, the last form fetched successfully is returned instead, so
        /// the form keeps rendering until someone reconnects.
        /// </summary>
        public async Task<FormDto> GetRealTimeForm(string id)
        {
            var cacheKey = RealTimeFormCacheKeyPrefix + id;

            var oauthConfiguration = _dynamicsConfigurationStorage.GetOAuthConfiguration();

            var forms = oauthConfiguration != null
                ? await Get<RealTimeFormDto>(oauthConfiguration.AccessToken, Constants.Modules.RealTimePath)
                : null;

            if (forms == null)
            {
                var cachedForm = _runtimeCache.GetCacheItem<FormDto>(cacheKey);

                _logger.LogWarning(
                    "Could not fetch Dynamics real-time form {FormId}. The access token may have expired; reconnect Dynamics in the backoffice. {Fallback}",
                    id,
                    cachedForm != null ? "Rendering the last version fetched." : "No earlier version is available, so the form is not rendered.");

                return cachedForm;
            }

            var form = forms.Value.FirstOrDefault(p => p.Id == id);
            if (form == null)
            {
                _runtimeCache.Clear(cacheKey);
                return null;
            }

            var formDto = new FormDto
            {
                RawHtml = form.FormHtml,
                StandaloneHtml = form.StandaloneHtml
            };

            _runtimeCache.Insert(cacheKey, () => formDto);

            return formDto;
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
                    ? JsonSerializer.Deserialize<IdentityDto>(result)
                    : new IdentityDto { IsAuthorized = false };
            }

            return new IdentityDto
            {
                IsAuthorized = true,
                UserId = JsonSerializer.Deserialize<JsonObject>(result)["UserId"].ToString()
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

            return JsonSerializer.Deserialize<ResponseDto<T>>(result);
        }
    }
}
