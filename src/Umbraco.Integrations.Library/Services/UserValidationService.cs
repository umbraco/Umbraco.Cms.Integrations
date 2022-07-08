using System.Linq;
using System.Threading.Tasks;

using Umbraco.Integrations.Library.Configuration;
using Umbraco.Integrations.Library.Interfaces;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Security;
#else
using System.Configuration;

using Umbraco.Core.Services;
#endif

namespace Umbraco.Integrations.Library.Services
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUserService _userService;

        private readonly LibrarySettings _settings;

#if NETCOREAPP
        private readonly IBackOfficeUserManager _backOfficeUserManager;

        public UserValidationService(IOptions<LibrarySettings> options, IBackOfficeUserManager backOfficeUserManager, IUserService userService)
        {
            _backOfficeUserManager = backOfficeUserManager;

            _userService = userService;

            _settings = options.Value;
        }
#else
        public UserValidationService(IUserService userService)
        {
            _userService = userService;

            _settings = new LibrarySettings(ConfigurationManager.AppSettings);
        }
#endif

        public async Task<bool> Validate(string username, string password, string apiKey, string userGroup = "")
        {
            if (!string.IsNullOrEmpty(apiKey))
                return apiKey == _settings.ApiKey;

#if NETCOREAPP
            var isUserValid =
                await _backOfficeUserManager.ValidateCredentialsAsync(username, password);
#else
            var isUserValid = Web.Composing.Current.UmbracoContext.Security.ValidateBackOfficeCredentials(username, password);
#endif

            if (!isUserValid) return false;

            if (!string.IsNullOrEmpty(_settings.UserGroupAlias))
            {
                var user = _userService.GetByUsername(username);

                return user != null && user.Groups.Any(p => p.Alias == _settings.UserGroupAlias);
            }

            return true;
        }
    }
}
