using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetSubscriptionHookController : ZapierControllerBase
    {
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        public GetSubscriptionHookController(
            IOptions<ZapierSettings> options,
            IUserValidationService userValidationService,
            ZapierSubscriptionHookService zapierSubscriptionHookService)
            : base(options, userValidationService) => _zapierSubscriptionHookService = zapierSubscriptionHookService;

        [HttpGet("subscription-hooks", Name = Constants.OperationIdentifiers.GetSubscriptionHooks)]
        [ProducesResponseType(typeof(IEnumerable<SubscriptionDto>), StatusCodes.Status200OK)]
        public IActionResult GetAll() => Ok(_zapierSubscriptionHookService.GetAll());
    }
}
