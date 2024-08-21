using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class CheckApiAccessController : ActiveCampaignControllerBase
    {
        public CheckApiAccessController(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory) : base(options, httpClientFactory)
        {
        }

        [HttpGet("api-access")]
        [ProducesResponseType(typeof(ApiAccessDto), StatusCodes.Status200OK)]
        public IActionResult CheckApiAccess() => Ok(new ApiAccessDto(Settings.BaseUrl, Settings.ApiKey));
    }
}
