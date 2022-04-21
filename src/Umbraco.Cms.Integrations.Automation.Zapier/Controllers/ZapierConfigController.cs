using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly ZapierService _zapierService;

        public ZapierConfigController(ZapierSubscriptionHookService zapierSubscriptionHookService, ZapierService zapierService)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;

            _zapierService = zapierService;
        }

        [HttpPost]
        public string Add([FromBody] ContentConfigDto dto)
        {
            var getByAliasResult = _zapierSubscriptionHookService.TryGetByAlias(dto.ContentTypeAlias, out _);
            if (getByAliasResult) return "A record for this content type already exists.";

            var result = _zapierSubscriptionHookService.Add(dto.ContentTypeAlias, dto.HookUrl);
            
            return result;
        } 

        [HttpGet]
        public IEnumerable<ContentConfigDto> GetAll() => _zapierSubscriptionHookService.GetAll();

        [HttpDelete]
        public string Delete(string contentTypeAlias, string hookUrl) => _zapierSubscriptionHookService.Delete(contentTypeAlias, hookUrl);

        [HttpPost]
        public async Task<string> TriggerWebHook([FromBody] ContentConfigDto dto)
        {
            return await _zapierService.TriggerAsync(dto.HookUrl,
                new Dictionary<string, string> { { Constants.Content.Name, dto.ContentTypeAlias } });
        }
    }
}
