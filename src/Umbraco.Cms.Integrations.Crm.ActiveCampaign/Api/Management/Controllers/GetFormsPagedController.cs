using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetFormsByPageController : ActiveCampaignControllerBase
    {
        public GetFormsByPageController(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory) : base(options, httpClientFactory)
        {
        }

        [HttpGet("forms")]
        [ProducesResponseType(typeof(FormCollectionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetForms([FromQuery] int? page = 1)
        {
            try
            {
                var client = HttpClientFactory.CreateClient(Constants.FormsHttpClient);

                var requestUriString = page == 1
                    ? $"{client.BaseAddress}{ApiPath}&limit={Constants.DefaultPageSize}"
                    : $"{client.BaseAddress}{ApiPath}&limit={Constants.DefaultPageSize}&offset={(page - 1) * Constants.DefaultPageSize}";

                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri(requestUriString),
                    Method = HttpMethod.Get
                };

                var response = await client.SendAsync(requestMessage);

                return await HandleResponseAsync<FormCollectionResponseDto>(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetailsBuilder()
                    .WithTitle(ex.Message)
                    .Build());
            }
        }
    }
}
