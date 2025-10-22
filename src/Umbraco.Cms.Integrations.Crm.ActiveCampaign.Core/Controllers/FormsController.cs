using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Models.Dtos;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmActiveCampaign")]
    public class FormsController : UmbracoAuthorizedApiController
    {
        private readonly ActiveCampaignSettings _settings;

        private readonly IHttpClientFactory _httpClientFactory;

        private const string ApiPath = "/api/3/forms";

        public FormsController(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory)
        {
            _settings = options.Value;

            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult CheckApiAccess() => new JsonResult(new ApiAccessDto(_settings.BaseUrl, _settings.ApiKey));

        [HttpGet]
        public async Task<IActionResult> GetForms(int page = 1, string searchQuery = "")
        {
            var client = _httpClientFactory.CreateClient(Constants.FormsHttpClient);
            
            var requestUriString = BuildRequestUri(client.BaseAddress.ToString(), page, searchQuery);

            var requestMessage = new HttpRequestMessage { 
                RequestUri = new Uri(requestUriString),
                Method = HttpMethod.Get 
            };

            var response = await client.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new JsonResult(new FormCollectionResponseDto
                {
                    Message = string.IsNullOrEmpty(content)
                        ? response.StatusCode == System.Net.HttpStatusCode.Forbidden
                            ? Constants.Resources.AuthorizationFailed : Constants.Resources.ApiAccessFailed
                        : JsonNode.Parse(content)["message"].ToString()
                });

            return new JsonResult(JsonSerializer.Deserialize<FormCollectionResponseDto>(content));
        }

        [HttpGet]
        public async Task<IActionResult> GetForm(string id)
        {
            var client = _httpClientFactory.CreateClient(Constants.FormsHttpClient);

            var response = await client.SendAsync(
                new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{client.BaseAddress}{ApiPath}/{id}")
                });

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new JsonResult(new FormResponseDto
                {
                    Message = string.IsNullOrEmpty(content)
                        ? response.StatusCode == System.Net.HttpStatusCode.Forbidden
                            ? Constants.Resources.AuthorizationFailed : Constants.Resources.ApiAccessFailed
                        : JsonNode.Parse(content)["message"].ToString()
                });

            return new JsonResult(JsonSerializer.Deserialize<FormResponseDto>(content));
        }

        private string BuildRequestUri(string baseAddress, int page, string searchQuery)
        {
            var uri = $"{baseAddress}{ApiPath}?limit={Constants.DefaultPageSize}";

            Dictionary<string, string> queryParamsDictionary = new Dictionary<string, string>();
            if (page > 1)
            {
                queryParamsDictionary.Add("offset", ((page - 1) * Constants.DefaultPageSize).ToString());
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                queryParamsDictionary.Add("search", HttpUtility.UrlEncode(searchQuery));
            }

            return queryParamsDictionary.Count == 0 
                ? uri
                : string.Format("{0}&{1}", uri, string.Join("&", queryParamsDictionary.Select(kvp => $"{kvp.Key}={kvp.Value}")));
        }
    }
}
