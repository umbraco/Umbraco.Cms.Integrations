#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
#else
using System.Web.Http;

using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;


#if NETCOREAPP
using Microsoft.Extensions.Options;
#else
using System.Configuration;
#endif

using static Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.GoogleComposer;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Controllers
{
    [PluginController("UmbracoCmsIntegrationsGoogleSearchConsole")]
    public class UrlInspectionApiController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        internal static Func<HttpClient> ClientFactory = () => s_client;

        private readonly IGoogleAuthorizationService _authorizationService;
        
        private readonly ITokenService _tokenService;

        private readonly GoogleSearchConsoleSettings _settings;

#if NETCOREAPP
        public UrlInspectionApiController(IOptions<GoogleSearchConsoleSettings> options, 
            ITokenService tokenService, AuthorizationImplementationFactory authorizationImplementationFactory)
#else
        public UrlInspectionApiController(ITokenService tokenService, AuthorizationImplementationFactory authorizationImplementationFactory)
#endif
        {
#if NETCOREAPP
            _settings = options.Value;
#else
            _settings = new GoogleSearchConsoleSettings(ConfigurationManager.AppSettings);
#endif

            _authorizationService = authorizationImplementationFactory(_settings.UseUmbracoAuthorization);

            _tokenService = tokenService;
        }

        [HttpGet]
        public OAuthConfigDto GetOAuthConfiguration() => new OAuthConfigDto
        {
            IsConnected = _tokenService.TryGetParameters(Constants.TokenDbKey, out _) &&
                          _tokenService.TryGetParameters(Constants.RefreshTokenDbKey, out _),
            AuthorizationUrl = _authorizationService.GetAuthorizationUrl()
        };

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] AuthorizationRequestDto authorizationRequestDto) => 
            await _authorizationService.GetAccessTokenAsync(authorizationRequestDto.Code);

        [HttpPost]
        public async Task<string> RefreshAccessToken() => await _authorizationService.RefreshAccessTokenAsync();

        [HttpPost]
        public void RevokeToken()
        {
            _tokenService.RemoveParameters(Constants.TokenDbKey);
            _tokenService.RemoveParameters(Constants.RefreshTokenDbKey);
        }

        [HttpPost]
        public async Task<ResponseDto> Inspect([FromBody] UrlInspectionDto dto)
        {
            _tokenService.TryGetParameters(Constants.TokenDbKey, out string accessToken);

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_settings.InspectUrl),
                Content = new StringContent(JsonConvert.SerializeObject(dto))
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ResponseDto>(content);
        }
    }
}
