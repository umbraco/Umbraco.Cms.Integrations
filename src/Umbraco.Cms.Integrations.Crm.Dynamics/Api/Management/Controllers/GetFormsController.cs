using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    public class GetFormsController : FormsControllerBase
    {
        public GetFormsController(
            IOptions<DynamicsSettings> options, 
            IDynamicsService dynamicsService, 
            IDynamicsConfigurationStorage dynamicsConfigurationStorage, 
            DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
            : base(options, dynamicsService, dynamicsConfigurationStorage, authorizationImplementationFactory)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForms(string module) =>
            Ok(await DynamicsService.GetForms((DynamicsModule)Enum.Parse(typeof(DynamicsModule), module)));
    }
}
