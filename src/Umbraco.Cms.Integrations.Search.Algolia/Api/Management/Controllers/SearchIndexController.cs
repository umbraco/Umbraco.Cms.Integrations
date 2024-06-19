using Algolia.Search.Models.Search;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
public class SearchIndexController : SearchControllerBase
{
    private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;
    private readonly IAlgoliaSearchService<SearchResponse<Record>> _searchService;

    public SearchIndexController(
        IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
        IAlgoliaSearchService<SearchResponse<Record>> searchService)
    {
        _indexStorage = indexStorage;
        _searchService = searchService;
    }

    [HttpGet("index/{indexId:int}/search")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    public IActionResult Search(int indexId, string query)
    {
        var index = _indexStorage.GetById(indexId);

        var searchResults = _searchService.Search(index.Name, query);

        var response = new Response
        {
            ItemsCount = searchResults.NbHits,
            PagesCount = searchResults.NbPages,
            ItemsPerPage = searchResults.HitsPerPage,
            Hits = searchResults.Hits.Select(p => p.Data.ToDictionary(x => x.Key, y => y.Value.ToString())).ToList()
        };

        return Ok(response);
    }
}
