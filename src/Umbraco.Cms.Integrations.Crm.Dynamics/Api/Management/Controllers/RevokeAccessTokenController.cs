using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    public class RevokeAccessTokenController : FormsControllerBase
    {
        public RevokeAccessTokenController(
            IOptions<DynamicsSettings> options, 
            IDynamicsService dynamicsService, 
            IDynamicsConfigurationStorage dynamicsConfigurationStorage, 
            DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
            : base(options, dynamicsService, dynamicsConfigurationStorage, authorizationImplementationFactory)
        {
        }

        [HttpDelete("revoke-access-token")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult RevokeAccessToken() => Ok(DynamicsConfigurationStorage.Delete());
    }
}
