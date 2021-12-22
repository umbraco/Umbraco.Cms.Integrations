using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Web.AuthorizationHub.Configuration;
using Umbraco.Cms.Integrations.Web.AuthorizationHub.Services;

namespace Umbraco.Cms.Integrations.Web.AuthorizationHub.Controllers
{
    [Route("authorizationhubapi/[controller]")]
    [ApiController]
    public class SemrushController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly SemrushSettings _settings;

        private readonly IDataSourceHandler _dataSourceHandler;

        private string SemrushDataSourcesPath => $"{_environment.WebRootPath}\\data";

        private string SemrushDataSourcesJsonPath => $"{SemrushDataSourcesPath}\\semrushDataSources.json";

        public SemrushController(IWebHostEnvironment environment, IHttpClientFactory httpClientFactory, IDataSourceHandler dataSourceHandler, IOptions<SemrushSettings> options)
        {
            _environment = environment;

            _httpClientFactory = httpClientFactory;

            _dataSourceHandler = dataSourceHandler.Build(SemrushDataSourcesPath);

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

        [HttpGet]
        [Route("import_datasources")]
        public async Task<JsonResult> ImportDatasources()
        {
            var ds = _dataSourceHandler.Read(SemrushDataSourcesJsonPath);
            if (!string.IsNullOrEmpty(ds))
            {
                var items = JsonSerializer.Deserialize<Dictionary<string, string>>(_dataSourceHandler.Read(SemrushDataSourcesJsonPath));

                return new JsonResult(items);
            }

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetStringAsync(_settings.DatabasesUrl);

            var config = AngleSharp.Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(r => r.Content(response));

            var dict = new Dictionary<string, string>();

            var el = document.GetElementById("databases");
            var tableBody = el.QuerySelectorAll("tbody");
            var rows = tableBody.Children("tr").ToArray();

            foreach (var r in rows)
            {
                var item = r.Children;

                dict.Add(
                    item[0].InnerHtml.Replace("\n", string.Empty).Replace("\t", string.Empty), 
                    item[1].InnerHtml.Replace("\n", string.Empty).Replace("\t", string.Empty));
            }

            _dataSourceHandler.Write(SemrushDataSourcesJsonPath, JsonSerializer.Serialize(dict));

            return new JsonResult(dict);
        }

    }
}
