using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUserService _userService;

        private readonly AppSettings _zapierSettings;

        private readonly ZapierFormsSettings _zapierFormsSettings;

        private readonly IBackOfficeUserManager _backOfficeUserManager;

        public UserValidationService(
            IOptions<AppSettings> options, 
            IOptions<ZapierFormsSettings> zapierFormsSettings,
            IBackOfficeUserManager backOfficeUserManager)
        {
            _backOfficeUserManager = backOfficeUserManager;

            _zapierSettings = options.Value;

            _zapierFormsSettings = zapierFormsSettings.Value;
        }

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
            {
                return ValidateByApiKey(apiKey);
            }

            return await ValidateByCredentials(username, password);
        }

        /// <summary>
        /// Validates user based on provided API key. 
        /// When both CMS and Forms packages are installed, both settings (CMS/Forms) will be compared.
        /// </summary>
        /// <param name="apiKey">Provided API key in the Zap authentication.</param>
        /// <returns></returns>
        private bool ValidateByApiKey(string apiKey) =>
            // Check API key from CMS and Forms settings. 
            (!string.IsNullOrEmpty(_zapierSettings.ApiKey) && _zapierSettings.ApiKey == apiKey)
                || (!string.IsNullOrEmpty(_zapierFormsSettings.ApiKey) && _zapierFormsSettings.ApiKey == apiKey);

        /// <summary>
        /// Validates user based on provided credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<bool> ValidateByCredentials(string username, string password)
        {
            var isUserValid =
                await _backOfficeUserManager.ValidateCredentialsAsync(username, password);

            if (!isUserValid) return false;

            if (!string.IsNullOrEmpty(_zapierSettings.UserGroupAlias))
            {
                var user = _userService.GetByUsername(username);

                return user != null && user.Groups.Any(p => p.Alias == _zapierSettings.UserGroupAlias);
            }

            return true;
        }
    }
}
