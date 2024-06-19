using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
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

        [HttpPost("index")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveIndex([FromBody] IndexConfiguration index)
        {
            _indexStorage.AddOrUpdate(new AlgoliaIndex
            {
                Id = index.Id,
                Name = index.Name,
                SerializedData = JsonSerializer.Serialize(index.ContentData
                        .Where(p => p.Selected && p.Properties.Any(q => q.Selected))),
                Date = DateTime.Now
            });

            var result = await _indexService.IndexExists(index.Name)
                ? Result.Ok()
                : await _indexService.PushData(index.Name);

            return Ok(result);
        }
    }
}
