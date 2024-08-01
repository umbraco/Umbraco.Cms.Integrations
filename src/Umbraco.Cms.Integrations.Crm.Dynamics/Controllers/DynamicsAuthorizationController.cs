using Umbraco.Cms.Integrations.Crm.Dynamics.Models;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.Common.Controllers;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Controllers
{
    public class DynamicsAuthorizationController : UmbracoApiController
    {
        [HttpGet]
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
