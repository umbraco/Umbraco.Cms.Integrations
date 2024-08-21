﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetFormByIdController : ActiveCampaignControllerBase
    {
        public GetFormByIdController(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory) : base(options, httpClientFactory)
        {
        }

        [HttpGet("forms/{id}")]
        [ProducesResponseType(typeof(FormResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForm(string id)
        {
            var client = HttpClientFactory.CreateClient(Constants.FormsHttpClient);

            var response = await client.SendAsync(
                new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{client.BaseAddress}{ApiPath}/{id}")
                });

            return await HandleResponseAsync(response);
        }
    }
}
