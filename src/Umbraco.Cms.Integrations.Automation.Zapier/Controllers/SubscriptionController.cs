using System.Linq;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.Controllers;
#else
using System.Web.Http;
using System.Configuration;
using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    /// <summary>
    /// Subscription API handling the ON/OFF trigger events in Zapier.
    /// </summary>
    public class SubscriptionController : UmbracoApiController
    {
        private readonly ZapierSettings Options;

        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        private readonly ZapierFormSubscriptionHookService _zapierFormSubscriptionHookService;

        private readonly IUserValidationService _userValidationService;

#if NETCOREAPP
        public SubscriptionController(IOptions<ZapierSettings> options, 
            ZapierSubscriptionHookService zapierSubscriptionHookService, 
            ZapierFormSubscriptionHookService zapierFormSubscriptionHookService,
            IUserValidationService userValidationService)
#else
        public SubscriptionController(
            ZapierSubscriptionHookService zapierSubscriptionHookService, 
            ZapierFormSubscriptionHookService zapierFormSubscriptionHookService,
            IUserValidationService userValidationService)
#endif
        {
#if NETCOREAPP
            Options = options.Value;
#else
            Options = new ZapierSettings(ConfigurationManager.AppSettings);
#endif

            _zapierSubscriptionHookService = zapierSubscriptionHookService;

            _zapierFormSubscriptionHookService = zapierFormSubscriptionHookService;

            _userValidationService = userValidationService;
        }

        [HttpPost]
        public bool UpdatePreferences([FromBody] SubscriptionDto dto)
        {
            string username = string.Empty;
            string password = string.Empty;

#if NETCOREAPP
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.UsernameHeaderKey, 
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.PasswordHeaderKey, 
                    out var passwordValues))
                password = passwordValues.First();
#else
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.UsernameHeaderKey,
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.PasswordHeaderKey,
                    out var passwordValues))
                password = passwordValues.First();
#endif

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            var isAuthorized = _userValidationService.Validate(username, password, Options.UserGroup).GetAwaiter().GetResult();
            if (!isAuthorized) return false;

            if (dto == null) return false;

            var result = dto.SubscribeHook
                ? _zapierSubscriptionHookService.Add(dto.ContentType, dto.HookUrl)
                : _zapierSubscriptionHookService.Delete(dto.ContentType, dto.HookUrl);

            return string.IsNullOrEmpty(result);
        }

        [HttpPost]
        public bool UpdateFormPreferences([FromBody] FormSubscriptionDto dto)
        {
            string username = string.Empty;
            string password = string.Empty;

#if NETCOREAPP
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.UsernameHeaderKey, 
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.PasswordHeaderKey, 
                    out var passwordValues))
                password = passwordValues.First();
#else
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.UsernameHeaderKey,
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.PasswordHeaderKey,
                    out var passwordValues))
                password = passwordValues.First();
#endif

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            var isAuthorized = _userValidationService.Validate(username, password, Options.UserGroup).GetAwaiter().GetResult();
            if (!isAuthorized) return false;

            if (dto == null) return false;

            var result = dto.SubscribeHook
                ? _zapierFormSubscriptionHookService.Add(dto.FormName, dto.HookUrl)
                : _zapierFormSubscriptionHookService.Delete(dto.FormName, dto.HookUrl);

            return string.IsNullOrEmpty(result);
        }
    }
}
