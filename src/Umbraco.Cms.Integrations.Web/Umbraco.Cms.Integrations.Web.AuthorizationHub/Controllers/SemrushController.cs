using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Web.AuthorizationHub.Configuration;

namespace Umbraco.Cms.Integrations.Web.AuthorizationHub.Controllers
{
    [Route("authorizationhubapi/[controller]")]
    [ApiController]
    public class SemrushController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly SemrushSettings _settings;

        public SemrushController(IHttpClientFactory httpClientFactory, IOptions<SemrushSettings> options)
        {
            _httpClientFactory = httpClientFactory;

            _settings = options.Value;
        }

        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "response";
        }

        [HttpPost]
        [Route("access_token")]
        public async Task<string> GetAccessToken()
        {
            var code = Request.Form["code"].ToString();

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_settings.AuthBaseUrl);

            var requestMessageDict = new Dictionary<string, string>();
            requestMessageDict.Add("client_id", _settings.ClientId);
            requestMessageDict.Add("client_secret", _settings.ClientSecret);
            requestMessageDict.Add("grant_type", "authorization_code");
            requestMessageDict.Add("code", code);
            requestMessageDict.Add("redirect_uri", _settings.RedirectUri);

            var request = new HttpRequestMessage(HttpMethod.Post, "oauth2/access_token")
            {
                Content = new FormUrlEncodedContent(requestMessageDict)
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var parsedResponse = await response.Content.ReadAsStringAsync();

                return parsedResponse;
            }

            return "error";
        }

        [HttpPost]
        [Route("refresh_access_token")]
        public async Task<string> RefreshAccessToken()
        {
            var refreshToken = Request.Form["refresh_token"].ToString();

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_settings.AuthBaseUrl);

            var requestMessageDict = new Dictionary<string, string>();
            requestMessageDict.Add("client_id", _settings.ClientId);
            requestMessageDict.Add("client_secret", _settings.ClientSecret);
            requestMessageDict.Add("grant_type", "refresh_token");
            requestMessageDict.Add("refresh_token", refreshToken);

            var request = new HttpRequestMessage(HttpMethod.Post, "oauth2/access_token")
            {
                Content = new FormUrlEncodedContent(requestMessageDict)
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var parsedResponse = await response.Content.ReadAsStringAsync();

                return parsedResponse;
            }

            return "error";
        }
    }
}
