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

namespace Umbraco.Cms.Integrations.Search.Algolia.Controllers.ManagementApi
{
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
        public async Task<IActionResult> DeleteIndex(int id)
        {
            try
            {
                var indexName = IndexStorage.GetById(id).Name;

                IndexStorage.Delete(id);

                await IndexService.DeleteIndex(indexName);

                return new JsonResult(Result.Ok());
            }
            catch (Exception ex)
            {
                return new JsonResult(Result.Fail(ex.Message));
            }
        }

    }
}
