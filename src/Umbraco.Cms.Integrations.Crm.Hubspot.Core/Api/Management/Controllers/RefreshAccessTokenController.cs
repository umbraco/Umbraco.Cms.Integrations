using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class RefreshAccessTokenController : HubspotFormsControllerBase
    {
        private readonly IHubspotAuthorizationService _authorizationService;

        public RefreshAccessTokenController(
            IOptions<HubspotSettings> settingsOptions,
            IHubspotAuthorizationService authorizationService) 
            : base(settingsOptions) => _authorizationService = authorizationService;

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var response = await _authorizationService.RefreshAccessTokenAsync();
            return Ok(response);
        }
    }
}
