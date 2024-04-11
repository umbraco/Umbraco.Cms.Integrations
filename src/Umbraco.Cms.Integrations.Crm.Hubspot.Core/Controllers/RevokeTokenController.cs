using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers;

public class RevokeTokenController : HubSpotFormsControllerBase
{
    private readonly ITokenService _tokenService;

    public RevokeTokenController(ITokenService tokenService) => _tokenService = tokenService;

    [HttpPost("revoke")]
    public void RevokeAccessToken()
    {
        _tokenService.RemoveParameters(Constants.AccessTokenDbKey);
        _tokenService.RemoveParameters(Constants.RefreshTokenDbKey);
    }
}
