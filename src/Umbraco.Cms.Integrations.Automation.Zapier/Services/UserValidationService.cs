using System.Linq;
using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
#else
using System.Configuration;

using Umbraco.Core.Services;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUserService _userService;

        private readonly ZapierSettings _zapierSettings;

#if NETCOREAPP
        private readonly IBackOfficeUserManager _backOfficeUserManager;

        public UserValidationService(IOptions<ZapierSettings> options, IBackOfficeUserManager backOfficeUserManager)
        {
            _backOfficeUserManager = backOfficeUserManager;

            _zapierSettings = options.Value;
        }
#else
        public UserValidationService(IUserService userService)
        {
            _userService = userService;

            _zapierSettings = new ZapierSettings(ConfigurationManager.AppSettings);
        }
#endif

        /// <summary>
        /// Allow access by validating API Key. If API key is missing, validate user credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public async Task<bool> Validate(string username, string password, string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
                return apiKey == _zapierSettings.ApiKey;

#if NETCOREAPP
            var isUserValid =
                await _backOfficeUserManager.ValidateCredentialsAsync(username, password);
#else
            var isUserValid = Web.Composing.Current.UmbracoContext.Security.ValidateBackOfficeCredentials(username, password);
#endif

            if (!isUserValid) return false;

            if (!string.IsNullOrEmpty(_zapierSettings.UserGroup))
            {
                var user = _userService.GetByUsername(username);

                return user != null && user.Groups.Any(p => p.Name == _zapierSettings.UserGroup);
            }

            return true;
        }
    }
}
