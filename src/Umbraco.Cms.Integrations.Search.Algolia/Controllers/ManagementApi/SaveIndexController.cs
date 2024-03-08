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
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Controllers.ManagementApi
{
    public class SaveIndexController : SearchControllerBase
    {
        public SaveIndexController(
            IAlgoliaIndexService indexService, 
            IAlgoliaSearchService<SearchResponse<Record>> searchService, 
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage, 
            IUserService userService, 
            IPublishedUrlProvider urlProvider, 
            IContentService contentService, 
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory, 
            IUmbracoContextFactory umbracoContextFactory, 
            ILogger<SaveIndexController> logger, 
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

        [HttpPost("index")]
        public async Task<IActionResult> SaveIndex([FromBody] IndexConfiguration index)
        {
            try
            {
                IndexStorage.AddOrUpdate(new AlgoliaIndex
                {
                    Id = index.Id,
                    Name = index.Name,
                    SerializedData = JsonSerializer.Serialize(index.ContentData
                        .Where(p => p.Selected && p.Properties.Any(q => q.Selected))),
                    Date = DateTime.Now
                });

                var result = await IndexService.IndexExists(index.Name)
                    ? Result.Ok()
                    : await IndexService.PushData(index.Name);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(Result.Fail(ex.Message));
            }
        }
    }
}
