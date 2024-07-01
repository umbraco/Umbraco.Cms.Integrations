using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class RevokeTokenController : HubspotFormsControllerBase
    {
        private readonly ITokenService _tokenService;

        public RevokeTokenController(
            IOptions<HubspotSettings> settingsOptions,
            ITokenService tokenService) 
            : base(settingsOptions) => _tokenService = tokenService;

        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RevokeAccessToken()
        {
            _tokenService.RemoveParameters(Constants.AccessTokenDbKey);
            _tokenService.RemoveParameters(Constants.RefreshTokenDbKey);

            return Ok();
        }
    }
}
