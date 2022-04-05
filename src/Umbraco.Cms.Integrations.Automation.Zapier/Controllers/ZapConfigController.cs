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

using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    [PluginController("UmbracoCmsIntegrationsAutomationZapier")]
    public class ZapConfigController : UmbracoAuthorizedApiController
    {
        private readonly IContentTypeService _contentTypeService;

        private readonly ZapConfigService _zapConfigService;

        private readonly ZapierService _zapierService;

        public ZapConfigController(IContentTypeService contentTypeService, ZapConfigService zapConfigService, ZapierService zapierService)
        {
            _contentTypeService = contentTypeService;

            _zapConfigService = zapConfigService;

            _zapierService = zapierService;
        }

        [HttpGet]
        public IEnumerable<ContentTypeDto> GetContentTypes()
        {
            var contentTypes = _contentTypeService.GetAll();

            var configEntities = _zapConfigService.GetAll().Select(p => p.ContentTypeName);

            return contentTypes
                .Where(p => !configEntities.Contains(p.Name))
                .OrderBy(p => p.Name)
                .Select(p => new ContentTypeDto
                    {
                        Name = p.Name
                    });
        }

        [HttpPost]
        public string Add([FromBody] ContentConfigDto dto)
        {
            var getByNameResult = _zapConfigService.GetByName(dto.ContentTypeName);
            if (getByNameResult != null) return "A record for this content type already exists.";

            var result = _zapConfigService.Add(dto);
            
            return result;
        } 

        [HttpGet]
        public IEnumerable<ContentConfigDto> GetAll() => _zapConfigService.GetAll();

        [HttpDelete]
        public string Delete(int id) => _zapConfigService.Delete(id);

        [HttpPost]
        public async Task<string> TriggerWebHook([FromBody] ContentConfigDto dto)
        {
            return await _zapierService.TriggerAsync(dto.WebHookUrl,
                new Dictionary<string, string> { { Constants.Content.Name, dto.ContentTypeName } });
        }
    }
}
