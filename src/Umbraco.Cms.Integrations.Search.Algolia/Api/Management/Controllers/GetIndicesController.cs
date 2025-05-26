using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiVersion("1.0")]
public class GetIndicesController : SearchControllerBase
{
    private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;

    public GetIndicesController(
        IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage) => _indexStorage = indexStorage;

    [HttpGet("index", Name = Constants.OperationIds.GetIndices)]
    [ProducesResponseType(typeof(IEnumerable<IndexConfiguration>), StatusCodes.Status200OK)]
    public IActionResult GetIndices()
    {
        var indices = _indexStorage.Get().Select(p => new IndexConfiguration
        {
            Id = p.Id,
            Name = p.Name,
            ContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(p.SerializedData) ?? []
        });

        return Ok(indices);
    }
}
