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
        public async Task<ResponseDto<IEnumerable<FormDto>>> GetForms()
        {
            var client = _httpClientFactory.CreateClient(Constants.FormsHttpClient);

            var response = await client.SendAsync(new HttpRequestMessage { Method = HttpMethod.Get });

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new ResponseDto<IEnumerable<FormDto>>
                {
                    Data = Enumerable.Empty<FormDto>(),
                    Message = String.IsNullOrEmpty(content)
                        ? response.StatusCode == System.Net.HttpStatusCode.Forbidden
                            ? Constants.Resources.AuthorizationFailed : Constants.Resources.ApiAccessFailed
                        : JsonNode.Parse(content)["message"].ToString()
                };

            return new ResponseDto<IEnumerable<FormDto>>
            {
                Data = string.IsNullOrEmpty(content)
                    ? Enumerable.Empty<FormDto>()
                    : JsonNode.Parse(content)["forms"].Deserialize<IEnumerable<FormDto>>(),
                Message = String.Empty
            };
        }

        [HttpGet]
        public async Task<ResponseDto<FormDto>?> GetForm(string id)
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
                return new ResponseDto<FormDto>
                {
                    Message = String.IsNullOrEmpty(content)
                        ? response.StatusCode == System.Net.HttpStatusCode.Forbidden
                            ? Constants.Resources.AuthorizationFailed : Constants.Resources.ApiAccessFailed
                        : JsonNode.Parse(content)["message"].ToString()
                };

            return new ResponseDto<FormDto>
            {
                Data = string.IsNullOrEmpty(content)
                ? new FormDto()
                : JsonNode.Parse(content)["form"].Deserialize<FormDto>(),
                Message = String.Empty
            };
        }

    }
}
