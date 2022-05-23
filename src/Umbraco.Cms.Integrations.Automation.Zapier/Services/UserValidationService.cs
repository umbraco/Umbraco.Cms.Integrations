using System.Linq;
using System.Threading.Tasks;

#if NETCOREAPP
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
#else
using Umbraco.Core.Services;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUserService _userService;


#if NETCOREAPP
        private readonly IBackOfficeUserManager _backOfficeUserManager;

        public UserValidationService(IBackOfficeUserManager backOfficeUserManager, IUserService userService)
        {
            _backOfficeUserManager = backOfficeUserManager;
        }
#else
        public UserValidationService(IUserService userService)
        {
            _userService = userService;
        }
#endif

        public async Task<bool> Validate(string username, string password, string userGroup)
        {
#if NETCOREAPP
            var isUserValid =
                await _backOfficeUserManager.ValidateCredentialsAsync(username, password);
#else
            var isUserValid = Web.Composing.Current.UmbracoContext.Security.ValidateBackOfficeCredentials(username, password);
#endif

            if (!isUserValid) return false;

            if (!string.IsNullOrEmpty(userGroup))
            {
                var user = _userService.GetByUsername(username);

                return user != null && user.Groups.Any(p => p.Name == userGroup);
            }

            return true;
        }
    }
}
