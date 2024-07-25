using System.Collections.Generic;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Asp.Versioning;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    /// <summary>
    /// Subscription API handling the ON/OFF trigger events in Zapier.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class SubscriptionController : ZapierControllerBase
    {
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        public SubscriptionController(IOptions<ZapierSettings> options,
            ZapierSubscriptionHookService zapierSubscriptionHookService,
            IUserValidationService userValidationService)
            : base(options, userValidationService)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;
        }

        [HttpPost("update-preferences")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult UpdatePreferences([FromBody] SubscriptionDto dto)
        {
            if (!IsAccessValid() || dto == null) 
                return NotFound();

            var result = dto.SubscribeHook
                ? _zapierSubscriptionHookService.Add(dto.EntityId, dto.Type, dto.HookUrl)
                : _zapierSubscriptionHookService.Delete(dto.EntityId, dto.Type, dto.HookUrl);

            return Ok(string.IsNullOrEmpty(result));
        }
    }
}
