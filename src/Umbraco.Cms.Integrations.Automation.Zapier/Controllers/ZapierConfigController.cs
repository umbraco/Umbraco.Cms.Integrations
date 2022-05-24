using System.Collections.Generic;

using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
#else
using System.Web.Http;

using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    [PluginController("UmbracoCmsIntegrationsAutomationZapier")]
    public class ZapierConfigController : UmbracoAuthorizedApiController
    {
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        public ZapierConfigController(
            ZapierSubscriptionHookService zapierSubscriptionHookService)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;
        }

        [HttpGet]
        public IEnumerable<SubscriptionDto> GetAll() => _zapierSubscriptionHookService.GetAll();

        [HttpGet]
        public bool IsFormsExtensionInstalled() => ReflectionHelper.IsFormsExtensionInstalled;
    }
}
