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

#if NETCOREAPP
        public SubscriptionController(IOptions<ZapierSettings> options, 
            ZapierSubscriptionHookService zapierSubscriptionHookService,
            IUserValidationService userValidationService)
            : base(options, userValidationService)
#else
        public SubscriptionController(
            ZapierSubscriptionHookService zapierSubscriptionHookService, 
            IUserValidationService userValidationService)
            : base(userValidationService)
#endif
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;
        }

        [HttpPost]
        public bool UpdatePreferences([FromBody] SubscriptionDto dto)
        {
            if (!IsUserValid() || dto == null) return false;

            var result = dto.SubscribeHook
                ? _zapierSubscriptionHookService.Add(dto.EntityId, dto.HookUrl)
                : _zapierSubscriptionHookService.Delete(dto.EntityId, dto.HookUrl);

            return string.IsNullOrEmpty(result);
        }
    }
}
