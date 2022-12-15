using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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

        public EntityController(IOptions<InriverSettings> options, IInriverService inriverService)
        {
            _settings = options.Value;

            _inriverService = inriverService;
        }

        [HttpGet]
        public IActionResult CheckApiAccess() => new JsonResult(
            !(string.IsNullOrEmpty(_settings.ApiBaseUrl) || string.IsNullOrEmpty(_settings.ApiKey))
                ? ServiceResponse<string>.Ok("Connected")
                : ServiceResponse<string>.Fail("Missing API configuration"));

        [HttpGet]
        public async Task<IActionResult> GetEntityTypes()
        {
            var results = await _inriverService.GetEntityTypes();

            return new JsonResult(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetEntitySummary(int id)
        {
            var results = await _inriverService.GetEntitySummary(id);
            if(results.Success)
            {
                var fields = await _inriverService.GetEntityFieldValues(id);
                if(fields.Success)
                {
                    results.Data.Fields = fields.Data;
                }
            }

            return new JsonResult(results);
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromBody] QueryRequest request)
        {
            var results = await _inriverService.Query(new QueryRequest
            {
                SystemCriteria = new List<Criterion>
                {
                   new Criterion(request.EntityTypeId)
                }
            });

            return new JsonResult(results);
        }
    }
}
