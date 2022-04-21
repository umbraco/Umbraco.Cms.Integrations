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
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;


namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Controllers
{
    [PluginController("UmbracoCmsIntegrationsGoogleSearchConsole")]
    public class UrlInspectionApiController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        internal static Func<HttpClient> ClientFactory = () => s_client;

        private readonly GoogleService _googleService;

        private readonly ITokenService _tokenService;

        public UrlInspectionApiController(GoogleService googleService, ITokenService tokenService)
        {
            _googleService = googleService;

            _tokenService = tokenService;
        }

        [HttpGet]
        public OAuthConfigDto GetOAuthConfiguration() => new OAuthConfigDto
        {
            IsConnected = _tokenService.TryGetParameters(_googleService.TokenDbKey, out _) &&
                          _tokenService.TryGetParameters(_googleService.RefreshTokenDbKey, out _),
            AuthorizationUrl = _googleService.GetAuthorizationUrl()
        };

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] AuthorizationRequestDto authorizationRequestDto)
        {
            var requestData = new Dictionary<string, string>
            {
                {"code", authorizationRequestDto.Code },
                {"client_id", _googleService.GetClientId() },
                {"redirect_uri", _googleService.GetRedirectUrl() },
                {"grant_type", "authorization_code" }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_googleService.GetAuthProxyTokenEndpoint()),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add(_googleService.ServiceHeaderKey.Key, _googleService.ServiceHeaderKey.Value);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                _tokenService.SaveParameters(_googleService.TokenDbKey, tokenDto.AccessToken);
                _tokenService.SaveParameters(_googleService.RefreshTokenDbKey, tokenDto.RefreshToken);

                return result;
            }

            return "error";
        }

        [HttpPost]
        public async Task<string> RefreshAccessToken()
        {
            _tokenService.TryGetParameters(_googleService.RefreshTokenDbKey, out string refreshToken);

            var requestData = new Dictionary<string, string>
            {
                {"client_id", _googleService.GetClientId()},
                {"refresh_token", refreshToken},
                {"grant_type", "refresh_token"}
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_googleService.GetAuthProxyTokenEndpoint()),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add(_googleService.ServiceHeaderKey.Key, _googleService.ServiceHeaderKey.Value);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                _tokenService.SaveParameters(_googleService.TokenDbKey, tokenDto.AccessToken);

                return result;
            }

            return "error";
        }

        [HttpPost]
        public void RevokeToken()
        {
            _tokenService.RemoveParameters(_googleService.TokenDbKey);
            _tokenService.RemoveParameters(_googleService.RefreshTokenDbKey);
        }

        [HttpPost]
        public async Task<ResponseDto> Inspect([FromBody] UrlInspectionDto dto)
        {
            _tokenService.TryGetParameters(_googleService.TokenDbKey, out string accessToken);

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_googleService.GetSearchConsoleInpectionUrl()),
                Content = new StringContent(JsonConvert.SerializeObject(dto))
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ResponseDto>(content);
        }
    }
}
