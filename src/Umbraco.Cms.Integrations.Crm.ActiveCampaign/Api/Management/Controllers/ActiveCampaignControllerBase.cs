using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}")]
    //[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public class ActiveCampaignControllerBase : Controller
    {
        protected readonly ActiveCampaignSettings Settings;

        protected readonly IHttpClientFactory HttpClientFactory;

        protected const string ApiPath = "/api/3/forms";

        public ActiveCampaignControllerBase(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory)
        {
            Settings = options.Value;

            HttpClientFactory = httpClientFactory;
        }

        protected async Task<IActionResult> HandleResponseAsync(HttpResponseMessage? httpResponse)
        {
            if (httpResponse is null)
            {
                return new EmptyResult();
            }

            var content = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                return Ok(new JsonResult(JsonSerializer.Deserialize<FormCollectionResponseDto>(content)));
            }

            var responseMessage = content.Contains("message") 
                ? JsonNode.Parse(content)!["message"]!.ToString() 
                : content;

            var message = httpResponse.StatusCode == HttpStatusCode.Forbidden
                ? Constants.Resources.AuthorizationFailed
                : string.IsNullOrEmpty(content)
                    ? Constants.Resources.ApiAccessFailed
                    : responseMessage;

            return StatusCode((int)httpResponse.StatusCode, message);
        }
    }
}
