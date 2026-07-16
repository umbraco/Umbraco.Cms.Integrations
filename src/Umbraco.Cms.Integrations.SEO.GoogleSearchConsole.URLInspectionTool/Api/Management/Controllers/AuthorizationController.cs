using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Api.Management.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ManagementApi.GoogleSearchConsoleGroupName)]
public class AuthorizationController : GoogleControllerBase
{
    public AuthorizationController(IOptions<GoogleSearchConsoleSettings> options, ITokenService tokenService, IHttpClientFactory clientFactory, GoogleComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
        : base(options, tokenService, clientFactory, authorizationImplementationFactory)
    {
    }

    [HttpGet("auth")]
    [ProducesResponseType(typeof(ContentResult), StatusCodes.Status200OK)]
    public IActionResult OAuth(string code)
    {
        return Ok(new ContentResult
        {
            Content = string.IsNullOrEmpty(code)
                ? JavascriptResponse.Fail("Authorization process failed.")
                : JavascriptResponse.Ok(code),
            ContentType = "text/html"
        });
    }

}
