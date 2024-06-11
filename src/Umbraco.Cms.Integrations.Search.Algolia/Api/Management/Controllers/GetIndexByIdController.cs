using Algolia.Search.Models.Search;
using Asp.Versioning;
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

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    public class GetIndexByIdController : SearchControllerBase
    {
        public GetIndexByIdController(
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

        [HttpGet("index/{id:int}")]
        public IActionResult GetIndexById(int id)
        {
            var index = IndexStorage.GetById(id);

            var indexConfiguration = new IndexConfiguration
            {
                Id = index.Id,
                Name = index.Name,
                ContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData)
            };

            return Ok(indexConfiguration);
        }
    }
}
