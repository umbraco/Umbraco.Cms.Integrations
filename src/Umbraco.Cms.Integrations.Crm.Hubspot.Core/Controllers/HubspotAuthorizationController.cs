using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models;

using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;

using Umbraco.Web.WebApi;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers
{
    public class HubspotAuthorizationController : UmbracoApiController
    {
        [HttpGet]
        public HttpResponseMessage OAuth(string code)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(string.IsNullOrEmpty(code)
                ? JavascriptResponse.Fail("Authorization process failed.")
                : JavascriptResponse.Ok(code));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
