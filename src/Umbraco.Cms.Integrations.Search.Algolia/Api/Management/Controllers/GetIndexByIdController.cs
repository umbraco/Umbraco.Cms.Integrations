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
public class GetIndexByIdController : SearchControllerBase
{
    private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;

    public GetIndexByIdController(IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage) => _indexStorage = indexStorage;

    [HttpGet("index/{id:int}", Name = Constants.OperationIds.GetSearchIndexById)]
    [ProducesResponseType(typeof(IndexConfiguration), StatusCodes.Status200OK)]
    public IActionResult GetIndexById(int id)
    {
        var index = _indexStorage.GetById(id);

        var indexConfiguration = new IndexConfiguration
        {
            Id = index.Id,
            Name = index.Name,
            ContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData) ?? []
        };

        return Ok(indexConfiguration);
    }
}
