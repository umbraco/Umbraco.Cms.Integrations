using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers
{
    [ApiController]
    [Route("/umbraco/api/hubspotauthorization")]
    public class HubspotAuthorizationController : Controller
    {
        [HttpGet("oauth")]
        public IActionResult OAuth(string code)
        {
            return new ContentResult
            {
                Content = string.IsNullOrEmpty(code)
                    ? JavascriptResponse.Fail("Authorization process failed.")
                    : JavascriptResponse.Ok(code),
                ContentType = "text/html"
            };
        }
    }
}
