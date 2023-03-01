using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Services;
using Umbraco.Cms.Web.BackOffice.ActionResults;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Controllers;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Controllers
{
    public class AuthorizationController : UmbracoApiController
    {
        private readonly IAprimoAuthorizationService _aprimoAuthorizationService;

        public AuthorizationController(IAprimoAuthorizationService aprimoAuthorizationService)
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
             
            var response = await _aprimoAuthorizationService.GetAccessToken(code);

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
