using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Models;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;


#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Security;
#else
using System.Web.Http;
using System.Configuration;

using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    /// <summary>
    /// When a Zapier user creates triggers using the Umbraco app from the Zapier App Directory, he needs to provide valid credentials for a backoffice account.
    /// </summary>
    public class AuthController : UmbracoApiController
    {
        private readonly ZapierSettings Options;

        private readonly IUserValidationService _userValidationService;

#if NETCOREAPP
        private readonly IBackOfficeUserManager _backOfficeUserManager;

        public AuthController(IBackOfficeUserManager backOfficeUserManager, IUserValidationService userValidationService, IOptions<ZapierSettings> options)
        {
            _backOfficeUserManager = backOfficeUserManager;
            
            _userValidationService = userValidationService;

            Options = options.Value;
        }
#else
        public AuthController(IUserValidationService userValidationService)
        {
            Options = new ZapierSettings(ConfigurationManager.AppSettings);

            _userValidationService = userValidationService;
        }
#endif

        [HttpPost]
        public async Task<bool> ValidateUser([FromBody] UserModel userModel) => await _userValidationService.Validate(userModel.Username, userModel.Password, Options.UserGroup);
    }
}