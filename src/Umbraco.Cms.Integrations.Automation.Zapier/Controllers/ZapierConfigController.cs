using System.Collections.Generic;
using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Core.Services;
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

        private readonly ZapierFormSubscriptionHookService _zapierFormSubscriptionHookService;

        private readonly ZapierService _zapierService;

        public ZapierConfigController(
            ZapierSubscriptionHookService zapierSubscriptionHookService, 
            ZapierFormSubscriptionHookService zapierFormSubscriptionHookService,
            ZapierService zapierService)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;

            _zapierFormSubscriptionHookService = zapierFormSubscriptionHookService;

            _zapierService = zapierService;
        }

        [HttpGet]
        public IEnumerable<ContentConfigDto> GetAll() => _zapierSubscriptionHookService.GetAll();

        [HttpGet]
        public bool IsFormsExtensionInstalled() => ReflectionHelper.IsFormsExtensionInstalled;

        [HttpGet]
        public IEnumerable<FormConfigDto> GetAllForms() => _zapierFormSubscriptionHookService.GetAll();


        [HttpPost]
        public async Task<string> TriggerWebHook([FromBody] ContentConfigDto dto)
        {
            return await _zapierService.TriggerAsync(dto.HookUrl,
                new Dictionary<string, string> { { Constants.Content.Name, dto.ContentTypeAlias } });
        }
    }
}
