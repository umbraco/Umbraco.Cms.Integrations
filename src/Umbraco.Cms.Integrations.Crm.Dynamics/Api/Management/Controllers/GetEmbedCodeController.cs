using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    public class GetEmbedCodeController : FormsControllerBase
    {
        public GetEmbedCodeController(
            IOptions<DynamicsSettings> options, 
            IDynamicsService dynamicsService, 
            IDynamicsConfigurationStorage dynamicsConfigurationStorage, 
            DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
            : base(options, dynamicsService, dynamicsConfigurationStorage, authorizationImplementationFactory)
        {
        }

        [HttpGet("embed-code")]
        [ProducesResponseType(typeof(Task<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmbedCode(string formId) => Ok(await DynamicsService.GetEmbedCode(formId));
    }
}
