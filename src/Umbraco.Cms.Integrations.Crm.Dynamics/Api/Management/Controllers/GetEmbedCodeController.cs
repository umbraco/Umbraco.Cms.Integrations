using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    public class GetEmbedCodeController : FormsControllerBase
    {
        public GetEmbedCodeController(IOptions<DynamicsSettings> options, DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService, DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) : base(options, dynamicsService, dynamicsConfigurationService, authorizationImplementationFactory)
        {
        }

        [HttpGet("embed-code")]
        [ProducesResponseType(typeof(Task<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmbedCode(string formId) => Ok(await DynamicsService.GetEmbedCode(formId));
    }
}
