using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    /// <summary>
    /// Subscription API handling the ON/OFF trigger events in Zapier.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class UpdateSubscriptionHookController : ZapierControllerBase
    {
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        public UpdateSubscriptionHookController(IOptions<ZapierSettings> options,
            ZapierSubscriptionHookService zapierSubscriptionHookService,
            IUserValidationService userValidationService)
            : base(options, userValidationService)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;
        }

        [HttpPost("subscription", Name = Constants.OperationIdentifiers.UpdateSubscription)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult UpdatePreferences([FromBody] SubscriptionDto dto)
        {
            if (!IsAccessValid())
                return Unauthorized();

            if (dto == null) return BadRequest();

            var result = dto.SubscribeHook
                ? _zapierSubscriptionHookService.Add(dto.EntityId, dto.Type, dto.HookUrl)
                : _zapierSubscriptionHookService.Delete(dto.EntityId, dto.Type, dto.HookUrl);

            return Ok(string.IsNullOrEmpty(result));
        }
    }
}
