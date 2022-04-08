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
using System.Threading.Tasks;
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
        public string GetAuthorizationUrl() => _googleService.GetAuthorizationUrl();

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] AuthorizationRequestDto authorizationRequestDto)
        {
            var requestData = new Dictionary<string, string>
            {
                {"code", authorizationRequestDto.Code},
                {"client_id", _googleService.GetClientId()},
                {"redirect_uri", "oauth/google"},
                {"grant_type", "authorization_code"}
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

                _tokenService.SaveParameters(_googleService.TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpPost]
        public void RevokeToken()
        {
            _tokenService.RemoveParameters(_googleService.TokenDbKey);
        }
    }
}
