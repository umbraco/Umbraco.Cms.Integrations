using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetFormsByIdController : ActiveCampaignControllerBase
    {
        public GetFormsByIdController(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory) : base(options, httpClientFactory)
        {
        }

        [HttpGet("forms/{id}")]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForm(string id)
        {
            var client = HttpClientFactory.CreateClient(Constants.FormsHttpClient);

            var response = await client.SendAsync(
                new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{client.BaseAddress}{ApiPath}/{id}")
                });

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var result = new JsonResult(new FormResponseDto
                {
                    Message = string.IsNullOrEmpty(content)
                        ? response.StatusCode == System.Net.HttpStatusCode.Forbidden
                            ? Constants.Resources.AuthorizationFailed : Constants.Resources.ApiAccessFailed
                        : JsonNode.Parse(content)["message"].ToString()
                });

                return Ok(result);
            }
                

            return Ok(new JsonResult(JsonSerializer.Deserialize<FormResponseDto>(content)));
        }
    }
}
