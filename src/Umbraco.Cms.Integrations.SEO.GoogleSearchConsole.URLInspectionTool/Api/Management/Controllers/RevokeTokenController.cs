using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Api.Management.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ManagementApi.GoogleSearchConsoleGroupName)]
public class RevokeTokenController : GoogleControllerBase
{
    public RevokeTokenController(IOptions<GoogleSearchConsoleSettings> options, ITokenService tokenService, IHttpClientFactory clientFactory, GoogleComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
        : base(options, tokenService, clientFactory, authorizationImplementationFactory)
    {
    }

    [HttpPost("oauth/revoke")]
    public IActionResult RevokeToken()
    {
        TokenService.RemoveParameters(Constants.TokenDbKey);
        TokenService.RemoveParameters(Constants.RefreshTokenDbKey);

        return Ok();
    }
}
