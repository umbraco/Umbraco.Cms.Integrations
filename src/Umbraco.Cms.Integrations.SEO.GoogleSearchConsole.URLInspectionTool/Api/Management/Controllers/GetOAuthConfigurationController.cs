using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Api.Management.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ManagementApi.GoogleSearchConsoleGroupName)]
public class GetOAuthConfigurationController : GoogleControllerBase
{
    public GetOAuthConfigurationController(IOptions<GoogleSearchConsoleSettings> options, ITokenService tokenService, IHttpClientFactory clientFactory, GoogleComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
        : base(options, tokenService, clientFactory, authorizationImplementationFactory)
    {
    }

    [HttpGet("oauth/configuration")]
    [ProducesResponseType(typeof(OAuthConfigDto), StatusCodes.Status200OK)]
    public IActionResult Get() => 
        Ok(new OAuthConfigDto
        {
            IsConnected = TokenService.TryGetParameters(Constants.TokenDbKey, out _) &&
                          TokenService.TryGetParameters(Constants.RefreshTokenDbKey, out _),
            AuthorizationUrl = AuthorizationService.GetAuthorizationUrl()
        });
}
