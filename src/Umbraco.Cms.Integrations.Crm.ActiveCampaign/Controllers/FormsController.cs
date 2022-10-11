using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Text.Json;
using System.Text.Json.Nodes;

using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Controllers
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
        public ApiAccessDto CheckApiAccess() => new ApiAccessDto(_settings.BaseUrl, _settings.ApiKey);

        [HttpGet]
        public async Task<IEnumerable<FormDto>?> GetForms()
        {
            var client = _httpClientFactory.CreateClient(Constants.FormsHttpClient);

            var response = await client.SendAsync(new HttpRequestMessage { Method = HttpMethod.Get });

            if(!response.IsSuccessStatusCode) return Enumerable.Empty<FormDto>();

            var content = await response.Content.ReadAsStringAsync();

            return string.IsNullOrEmpty(content)
                ? Enumerable.Empty<FormDto>()
                : JsonNode.Parse(content)["forms"].Deserialize<IEnumerable<FormDto>>();
        }

    }
}
