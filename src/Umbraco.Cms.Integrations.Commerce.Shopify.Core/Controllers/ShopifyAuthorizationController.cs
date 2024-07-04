using Umbraco.Cms.Integrations.Commerce.Shopify.Models;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.Common.Controllers;
#else
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;

using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Controllers
{
    public class ShopifyAuthorizationController : UmbracoApiController
    {
        [HttpGet]
#if NETCOREAPP
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
#else
        public HttpResponseMessage OAuth(string code)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(string.IsNullOrEmpty(code)
                ? JavascriptResponse.Fail("Authorization process failed.")
                : JavascriptResponse.Ok(code));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
         }
#endif
    }
}
