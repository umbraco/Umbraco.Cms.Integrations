using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Services;
using Umbraco.Cms.Web.Common.Controllers;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Controllers
{
    // TODO: update route before package release
    [Route("/umbraco/api/authorization/oauth")]
    public class AprimoAuthorizationController : UmbracoApiController
    {
        private readonly IAprimoAuthorizationService _aprimoAuthorizationService;

        public AprimoAuthorizationController(IAprimoAuthorizationService aprimoAuthorizationService)
        {
            _aprimoAuthorizationService = aprimoAuthorizationService;   
        }

        [HttpGet]
        public async Task<IActionResult> OAuth(string code)
        {
            if(string.IsNullOrEmpty(code))
            {
                return new ContentResult
                {
                    Content = JavascriptResponse.Fail("Authorization process failed."),
                    ContentType = "text/html"
                };
            } 
             
            var response = await _aprimoAuthorizationService.GetAccessTokenAsync(code);

            return new ContentResult
            {
                Content =  response.Contains("Error")
                    ? JavascriptResponse.Fail(response)
                    : JavascriptResponse.Ok(),
                ContentType = "text/html"
            };
        }
    }
}
