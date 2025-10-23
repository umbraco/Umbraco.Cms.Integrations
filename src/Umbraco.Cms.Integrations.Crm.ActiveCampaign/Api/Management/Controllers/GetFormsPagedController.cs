using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
        public async Task<IActionResult> GetForms([FromQuery] int? page = 1, string? searchQuery = "")
        {
            try
            {
                var client = HttpClientFactory.CreateClient(Constants.FormsHttpClient);

                var requestUriString = BuildRequestUri(client.BaseAddress.ToString(), page ?? 1, searchQuery);

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

        private string BuildRequestUri(string baseAddress, int page, string searchQuery)
        {
            var uri = $"{baseAddress}{ApiPath}?limit={Constants.DefaultPageSize}";
            
            if (page > 1)
            {
                uri = QueryHelpers.AddQueryString(uri, "offset", ((page - 1) * Constants.DefaultPageSize).ToString());
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                uri = QueryHelpers.AddQueryString(uri, "search", searchQuery);
            }

            return uri;
        }
    }
}
