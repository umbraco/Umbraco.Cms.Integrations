using Algolia.Search.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Controllers.ManagementApi
{
    public class GetIndicesController : SearchControllerBase
    {
        public GetIndicesController(
            IAlgoliaIndexService indexService, 
            IAlgoliaSearchService<SearchResponse<Record>> searchService, 
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage, 
            IUserService userService, IPublishedUrlProvider urlProvider, 
            IContentService contentService, 
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory, 
            IUmbracoContextFactory umbracoContextFactory, 
            ILogger<GetIndicesController> logger, 
            IRecordBuilderFactory recordBuilderFactory) 
            : base(
                  indexService, 
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

        [HttpGet("index")]
        public IActionResult GetIndices()
        {
            var results = IndexStorage.Get().Select(p => new IndexConfiguration
            {
                Id = p.Id,
                Name = p.Name,
                ContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(p.SerializedData)
            });

            return new JsonResult(results);
        }
    }
}
