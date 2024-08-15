using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class OAuthController : DynamicsControllerBase
    {
        [HttpGet("authorization")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
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
