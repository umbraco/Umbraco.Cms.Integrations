using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetAccessTokenController : HubspotFormsControllerBase
    {
        private readonly IHubspotAuthorizationService _authorizationService;

        public GetAccessTokenController(
            IOptions<HubspotSettings> settingsOptions,
            IHubspotAuthorizationService authorizationService) : base(settingsOptions) => _authorizationService = authorizationService;

        [HttpPost("access-token")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccessToken([FromBody] OAuthRequestDto authRequestDto)
        {
            var accessToken = await _authorizationService.GetAccessTokenAsync(authRequestDto.Code);
            return Ok(accessToken);
        }
    }
}
