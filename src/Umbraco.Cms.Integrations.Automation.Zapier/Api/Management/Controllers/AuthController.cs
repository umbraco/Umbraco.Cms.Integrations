using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Automation.Zapier.Models;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Microsoft.AspNetCore.Http;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    /// <summary>
    /// When a Zapier user creates triggers using the Umbraco app from the Zapier App Directory, they need to provide valid credentials for a backoffice account.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class AuthController : ZapierControllerBase
    {

        public AuthController(IOptions<ZapierSettings> options, IUserValidationService userValidationService) : base(options, userValidationService)
        {
        }

        [HttpPost("validate-user")]
        [ProducesResponseType(typeof(Task<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateUser([FromBody] UserModel userModel)
        {
            var result = await _userValidationService.Validate(userModel.Username, userModel.Password, userModel.ApiKey);
            return Ok(result);
        }
    }
}