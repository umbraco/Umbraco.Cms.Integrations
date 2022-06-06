using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Automation.Zapier.Models;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.Common.Controllers;
#else
using System.Web.Http;

using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    /// <summary>
    /// When a Zapier user creates triggers using the Umbraco app from the Zapier App Directory, they need to provide valid credentials for a backoffice account.
    /// </summary>
    public class AuthController : UmbracoApiController
    {
        private readonly IUserValidationService _userValidationService;

        public AuthController(IUserValidationService userValidationService)
        {
            _userValidationService = userValidationService;
        }

        [HttpPost]
        public async Task<bool> ValidateUser([FromBody] UserModel userModel) => await _userValidationService.Validate(userModel.Username, userModel.Password, userModel.ApiKey);
    }
}