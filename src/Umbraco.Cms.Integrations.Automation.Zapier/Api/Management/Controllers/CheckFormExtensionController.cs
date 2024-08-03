using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class CheckFormExtensionController : ZapierControllerBase
    {
        public CheckFormExtensionController(IOptions<ZapierSettings> options, IUserValidationService userValidationService) 
            : base(options, userValidationService)
        {
        }

        [HttpGet("check-form-extension")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult CheckFormsExtensionInstalled() => Ok(ReflectionHelper.IsFormsExtensionInstalled);
    }
}
