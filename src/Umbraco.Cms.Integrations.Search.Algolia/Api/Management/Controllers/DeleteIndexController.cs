using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiVersion("1.0")]
public class DeleteIndexController : SearchControllerBase
{
    private readonly IAlgoliaIndexService _indexService;
    private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;

    public DeleteIndexController(
        IAlgoliaIndexService indexService, 
        IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage)
    {
        _indexService = indexService;
        _indexStorage = indexStorage;
    }

    [HttpDelete("index/{id:int}", Name = Constants.OperationIds.DeleteSearchIndex)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteIndex(int id)
    {
        var indexName = _indexStorage.GetById(id).Name;

        _indexStorage.Delete(id);

        await _indexService.DeleteIndex(indexName);

        return Ok(Result.Ok());
    }
}
