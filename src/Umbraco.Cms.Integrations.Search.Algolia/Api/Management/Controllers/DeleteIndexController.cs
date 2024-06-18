using Algolia.Search.Models.Search;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
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
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class DeleteIndexController : SearchControllerBase
    {
        public DeleteIndexController(
            IAlgoliaIndexService indexService, 
            IAlgoliaSearchService<SearchResponse<Record>> searchService, 
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage, 
            IUserService userService, IPublishedUrlProvider urlProvider, 
            IContentService contentService, 
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory, 
            IUmbracoContextFactory umbracoContextFactory, 
            ILogger<DeleteIndexController> logger, 
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

        [HttpDelete("index/{id:int}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteIndex(int id)
        {
            var indexName = IndexStorage.GetById(id).Name;

            IndexStorage.Delete(id);

            await IndexService.DeleteIndex(indexName);

            return Ok(Result.Ok());
        }

    }
}
