using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models;
using Umbraco.Cms.Web.Common.Controllers;

using Microsoft.AspNetCore.Mvc;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers
{
    public class HubspotAuthorizationController : UmbracoApiController
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
