using Algolia.Search.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers
{
    public class SearchIndexController : SearchControllerBase
    {
        public SearchIndexController(
            IAlgoliaIndexService indexService,
            IAlgoliaSearchService<SearchResponse<Record>> searchService,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IUserService userService, IPublishedUrlProvider urlProvider,
            IContentService contentService,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory,
            IUmbracoContextFactory umbracoContextFactory,
            ILogger<SearchIndexController> logger,
            IRecordBuilderFactory recordBuilderFactory)
            : base(indexService,
                  searchService,
                  indexStorage,
                  userService,
                  urlProvider,
                  contentService,
                  algoliaSearchPropertyIndexValueFactory,
                  umbracoContextFactory,
                  logger,
                  recordBuilderFactory)
        {
        }

        [HttpGet("index/{indexId:int}/search")]
        public IActionResult Search(int indexId, string query)
        {
            var index = IndexStorage.GetById(indexId);

            var searchResults = SearchService.Search(index.Name, query);

            var response = new Response
            {
                ItemsCount = searchResults.NbHits,
                PagesCount = searchResults.NbPages,
                ItemsPerPage = searchResults.HitsPerPage,
                Hits = searchResults.Hits.Select(p => p.Data.ToDictionary(x => x.Key, y => y.Value.ToString())).ToList()
            };

            return new JsonResult(response);
        }
    }
}
