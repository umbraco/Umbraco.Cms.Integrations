using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    public class GetFormsController : FormsControllerBase
    {
        public GetFormsController(IOptions<DynamicsSettings> options, DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService, DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) : base(options, dynamicsService, dynamicsConfigurationService, authorizationImplementationFactory)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForms(string module) =>
            Ok(await DynamicsService.GetForms((DynamicsModule)Enum.Parse(typeof(DynamicsModule), module)));
    }
}
