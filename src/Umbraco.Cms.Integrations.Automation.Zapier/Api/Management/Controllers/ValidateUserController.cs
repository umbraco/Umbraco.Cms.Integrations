using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Models;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    /// <summary>
    /// When a Zapier user creates triggers using the Umbraco app from the Zapier App Directory, they need to provide valid credentials for a backoffice account.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class ValidateUserController : ZapierControllerBase
    {
        public ValidateUserController(IOptions<ZapierSettings> options, IUserValidationService userValidationService) : base(options, userValidationService)
        {
        }

        [HttpPost("validate-user")]
        [ProducesResponseType(typeof(Task<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Validate([FromBody] UserModel userModel)
        {
            var result = await _userValidationService.Validate(userModel.Username, userModel.Password, userModel.ApiKey);
            return Ok(result);
        }
    }
}