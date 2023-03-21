using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.Common.Controllers;
#else
using System.Web.Mvc;

using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Controllers
{
    public class GoogleSearchConsoleAuthorizationController : UmbracoApiController
    {
        [HttpGet]
#if NETCOREAPP
        public IActionResult OAuth(string code)
#else
        public ActionResult OAuth(string code)
#endif
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
