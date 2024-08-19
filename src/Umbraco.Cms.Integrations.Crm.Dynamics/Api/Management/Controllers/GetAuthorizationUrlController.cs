using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    public class GetAuthorizationUrlController : FormsControllerBase
    {
        public GetAuthorizationUrlController(
            IOptions<DynamicsSettings> options, 
            IDynamicsService dynamicsService, 
            IDynamicsConfigurationStorage dynamicsConfigurationStorage, 
            DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
            : base(options, dynamicsService, dynamicsConfigurationStorage, authorizationImplementationFactory)
        {
        }

        [HttpGet("authorization-url")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetAuthorizationUrl()
        {
            var url = AuthorizationService.GetAuthorizationUrl();
            return Ok(url);
        }
    }
}
