using Algolia.Search.Exceptions;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Umbraco.Cms.Integrations.Search.Algolia.Extensions;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    public class SaveIndexController : SearchControllerBase
    {
        private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;
        private readonly IAlgoliaIndexService _indexService;

        public SaveIndexController(
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IAlgoliaIndexService indexService)
        {
            _indexStorage = indexStorage;
            _indexService = indexService;
        }

        [HttpPost("index", Name = Constants.OperationIds.SaveIndex)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveIndex([FromBody] IndexConfiguration index)
        {
            try
            {
                var result = await _indexService.IndexExists(index.Name)
                    ? Result.Ok()
                    : await _indexService.PushData(index.Name);

                _indexStorage.AddOrUpdate(new AlgoliaIndex
                {
                    Id = index.Id,
                    Name = index.Name,
                    SerializedData = JsonSerializer.Serialize(index.ContentData.FilterByPropertySelected()),
                    Date = DateTime.Now
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return OperationStatusResult(OperationStatus.ApiException, ex.Message);
            }
        }
    }
}
