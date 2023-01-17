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
            var result = await _inriverService.GetEntityTypes();

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromBody] QueryRequest request)
        {
            var result = await _inriverService.Query(new QueryRequest
            {
                SystemCriteria = new List<Criterion>
                {
                   new Criterion(request.EntityTypeId)
                }
            });

            if(result.Failure) return new JsonResult(ServiceResponse<IEnumerable<EntityData>>.Fail(result.Error));

            var data = await _inriverService.FetchData(new FetchDataRequest
            {
                EntityIds = result.Data.EntityIds,
                FieldTypeIds = request.FieldTypeIds
            });

            var dataResult = new List<EntityData>
            {
                new EntityData { 
                    EntityId = 7, 
                    Summary = new EntitySummary{ DisplayName = "Task1" , Description = "This is my task"} ,
                    Fields = new List<FieldValue>
                    {
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" }
                    }
                },
                new EntityData {
                    EntityId = 7,
                    Summary = new EntitySummary{ DisplayName = "Task2" , Description = "This is my task"} ,
                    Fields = new List<FieldValue>
                    {
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" }
                    }
                },
                new EntityData {
                    EntityId = 7,
                    Summary = new EntitySummary{ DisplayName = "Task3" , Description = "This is my task"} ,
                    Fields = new List<FieldValue>
                    {
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" }
                    }
                },
                new EntityData {
                    EntityId = 7,
                    Summary = new EntitySummary{ DisplayName = "Task4" , Description = "This is my task"} ,
                    Fields = new List<FieldValue>
                    {
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" }
                    }
                },
                new EntityData {
                    EntityId = 7,
                    Summary = new EntitySummary{ DisplayName = "Task5" , Description = "This is my task"} ,
                    Fields = new List<FieldValue>
                    {
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" }
                    }
                },
                new EntityData {
                    EntityId = 7,
                    Summary = new EntitySummary{ DisplayName = "Task6" , Description = "This is my task"} ,
                    Fields = new List<FieldValue>
                    {
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" }
                    }
                },
                new EntityData {
                    EntityId = 7,
                    Summary = new EntitySummary{ DisplayName = "Task7" , Description = "This is my task"} ,
                    Fields = new List<FieldValue>
                    {
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" },
                        new FieldValue { FieldTypeId = "Test", Value = "Task" }
                    }
                }

            };

            return new JsonResult(ServiceResponse<IEnumerable<EntityData>>.Ok(dataResult));
        }

        [HttpPost]
        public async Task<IActionResult> FetchData([FromBody] FetchDataRequest request)
        {
            var result = await _inriverService.FetchData(request);

            return new JsonResult(result);
        }
    }
}
