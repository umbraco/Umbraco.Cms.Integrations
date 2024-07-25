using System.Collections.Generic;
using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Attributes;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Asp.Versioning;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class ConfigController : ZapierControllerBase
    {
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        public ConfigController(IOptions<ZapierSettings> options, IUserValidationService userValidationService, ZapierSubscriptionHookService zapierSubscriptionHookService) : base(options, userValidationService)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<SubscriptionDto>), StatusCodes.Status200OK)]
        public IActionResult GetAll() => Ok(_zapierSubscriptionHookService.GetAll());

        [HttpGet("form-extension-installed")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult IsFormsExtensionInstalled() => Ok(ReflectionHelper.IsFormsExtensionInstalled);
    }
}
