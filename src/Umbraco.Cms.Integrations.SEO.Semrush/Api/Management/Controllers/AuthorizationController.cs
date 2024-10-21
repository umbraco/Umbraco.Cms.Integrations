using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.SemrushGroupName)]
    public class AuthorizationController : SemrushControllerBase
    {
        public AuthorizationController(
            IOptions<SemrushSettings> options,
            IWebHostEnvironment webHostEnvironment,
            ISemrushTokenService semrushTokenService,
            ICacheHelper cacheHelper,
            TokenBuilder tokenBuilder,
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IHttpClientFactory httpClientFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory, httpClientFactory)
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
}
