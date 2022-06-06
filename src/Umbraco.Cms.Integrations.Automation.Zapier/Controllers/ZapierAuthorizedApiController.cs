using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Web.Common.Controllers;
#else
using System.Configuration;

using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    public class ZapierAuthorizedApiController : UmbracoApiController
    {
        private readonly ZapierSettings Options;

        private readonly IUserValidationService _userValidationService;

#if NETCOREAPP
        public ZapierAuthorizedApiController(IOptions<ZapierSettings> options, IUserValidationService userValidationService)
#else
        public ZapierAuthorizedApiController(IUserValidationService userValidationService)
#endif
        {
#if NETCOREAPP
            Options = options.Value;
#else
            Options = new ZapierSettings(ConfigurationManager.AppSettings);
#endif

            _userValidationService = userValidationService;
        }

        public bool IsAccessValid()
        {
            string username = string.Empty;
            string password = string.Empty;
            string apiKey = string.Empty;

#if NETCOREAPP
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.UsernameHeaderKey,
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.PasswordHeaderKey,
                    out var passwordValues))
                password = passwordValues.First();
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.ApiKeyHeaderKey,
                    out var apiKeyValues))
                apiKey = apiKeyValues.First();
#else
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.UsernameHeaderKey,
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.PasswordHeaderKey,
                    out var passwordValues))
                password = passwordValues.First();
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.ApiKeyHeaderKey,
                    out var apiKeyValues))
                apiKey = apiKeyValues.First();
#endif

            if (string.IsNullOrEmpty(apiKey) && (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))) return false;

            if (!string.IsNullOrEmpty(apiKey))
                return apiKey == Options.ApiKey;

            var isAuthorized = _userValidationService.Validate(username, password, Options.UserGroupAlias).GetAwaiter()
                .GetResult();
            if (!isAuthorized) return false;

            return true;
        }
    }
}
