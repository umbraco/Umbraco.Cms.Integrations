using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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

        public FormsController(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory)
        {
            _settings = options.Value;

            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult CheckApiAccess() => new JsonResult(new ApiAccessDto(_settings.BaseUrl, _settings.ApiKey));

        [HttpGet]
        public async Task<IActionResult> GetForms()
        {
            var client = _httpClientFactory.CreateClient(Constants.FormsHttpClient);

            var response = await client.SendAsync(new HttpRequestMessage { Method = HttpMethod.Get });

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
                    RequestUri = new Uri(client.BaseAddress + "/" + id)
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

    }
}
