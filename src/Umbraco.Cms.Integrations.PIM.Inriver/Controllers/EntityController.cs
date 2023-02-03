using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.PIM.Inriver.Configuration;
using Umbraco.Cms.Integrations.PIM.Inriver.Models;
using Umbraco.Cms.Integrations.PIM.Inriver.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Controllers
{
    [PluginController("UmbracoCmsIntegrationsPimInriver")]
    public class EntityController : UmbracoAuthorizedApiController
    {
        private readonly InriverSettings _settings;

        private readonly IInriverService _inriverService;

        private readonly ILocalizationService _localizationService;

        public EntityController(IOptions<InriverSettings> options, IInriverService inriverService, ILocalizationService localizationService)
        {
            _settings = options.Value;

            _inriverService = inriverService;

            _localizationService = localizationService;
        }

        [HttpGet]
        public IActionResult CheckApiAccess() => new JsonResult(
            !(string.IsNullOrEmpty(_settings.ApiBaseUrl) || string.IsNullOrEmpty(_settings.ApiKey))
                ? ServiceResponse<string>.Ok("Connected")
                : ServiceResponse<string>.Fail("Missing API configuration"));

        [HttpGet]
        public async Task<IActionResult> GetEntityTypes()
        {
            var result = await _inriverService.GetEntityTypes();

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromBody] QueryRequest request)
        {
            var language = _localizationService.GetLanguageByIsoCode(
                string.IsNullOrEmpty(request.Culture) 
                    ? _localizationService.GetDefaultLanguageIsoCode()
                    : request.Culture);

            var result = await _inriverService.Query(new QueryRequest
            {
                SystemCriteria = new List<Criterion>
                {
                   new Criterion(request.EntityTypeId)
                }
            });

            if (result.Failure) return new JsonResult(ServiceResponse<IEnumerable<EntityData>>.Fail(result.Error));

            var dataResult = await _inriverService.FetchData(new FetchDataRequest
            {
                EntityIds = result.Data.EntityIds,
                FieldTypeIds = string.Empty,
                Outbound = request.LinkedTypes != null && request.LinkedTypes.Length > 0 
                    ? new Outbound { LinkTypeIds = string.Join(",", request.LinkedTypes) } 
                    : null
            });

            foreach (var item in dataResult.Data)
            {
                item.Fields = item.Fields.Where(p => request.FieldTypes.Any(q => q.Id == p.FieldTypeId));
                foreach (var field in item.Fields)
                {
                    if (field.ValueDictionary != null)
                    { 
                        field.Value = field.ValueDictionary.First(p => p.Key == language.CultureInfo.TwoLetterISOLanguageName).Value;
                    }
                }
            }

            return new JsonResult(dataResult);
        }

        [HttpPost]
        public async Task<IActionResult> FetchData([FromBody] FetchDataRequest request)
        {
            var result = await _inriverService.FetchData(request);

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> FetchEntityData(int entityId)
        {
            var result = await _inriverService.FetchData(new FetchDataRequest
            {
                EntityIds = new int[] { entityId },
                FieldTypeIds = string.Empty
            });

            return new JsonResult(result);
        }
    }
}
