using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
#else
using System.Web.Http;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    /// <summary>
    /// Subscription API handling the ON/OFF trigger events in Zapier.
    /// </summary>
    public class SubscriptionController : ZapierAuthorizedApiController
    {
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        private readonly ZapierFormSubscriptionHookService _zapierFormSubscriptionHookService;

#if NETCOREAPP
        public SubscriptionController(IOptions<ZapierSettings> options, 
            ZapierSubscriptionHookService zapierSubscriptionHookService, 
            ZapierFormSubscriptionHookService zapierFormSubscriptionHookService,
            IUserValidationService userValidationService)
            : base(options, userValidationService)
#else
        public SubscriptionController(
            ZapierSubscriptionHookService zapierSubscriptionHookService, 
            ZapierFormSubscriptionHookService zapierFormSubscriptionHookService,
            IUserValidationService userValidationService)
            : base(userValidationService)
#endif
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;

            _zapierFormSubscriptionHookService = zapierFormSubscriptionHookService;
        }

        [HttpPost]
        public bool UpdatePreferences([FromBody] SubscriptionDto dto)
        {
            if (!IsUserValid() || dto == null) return false;

            var result = dto.SubscribeHook
                ? _zapierSubscriptionHookService.Add(dto.ContentType, dto.HookUrl)
                : _zapierSubscriptionHookService.Delete(dto.ContentType, dto.HookUrl);

            return string.IsNullOrEmpty(result);
        }

        [HttpPost]
        public bool UpdateFormPreferences([FromBody] FormSubscriptionDto dto)
        {
            if (!IsUserValid() || dto == null) return false;

            var result = dto.SubscribeHook
                ? _zapierFormSubscriptionHookService.Add(dto.FormName, dto.HookUrl)
                : _zapierFormSubscriptionHookService.Delete(dto.FormName, dto.HookUrl);

            return string.IsNullOrEmpty(result);
        }
    }
}
