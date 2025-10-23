using System.Diagnostics;
using Algolia.Search.Models.Search;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Implement;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Controllers
{
    [PluginController("UmbracoCmsIntegrationsSearchAlgolia")]
    public class SearchController : UmbracoAuthorizedApiController
    {
        private readonly IAlgoliaIndexService _indexService;

        private readonly IAlgoliaSearchService<SearchResponse<Record>> _searchService;

        private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;

        private readonly IUserService _userService;

        private readonly IPublishedUrlProvider _urlProvider;

        private readonly IContentService _contentService;

        private readonly IAlgoliaSearchPropertyIndexValueFactory _algoliaSearchPropertyIndexValueFactory;

        private readonly IAlgoliaGeolocationProvider _algoliaGeolocationProvider;

        private readonly IUmbracoContextFactory _umbracoContextFactory;

        private readonly ILogger<SearchController> _logger;

        private readonly IRecordBuilderFactory _recordBuilderFactory;

        public SearchController(
            IAlgoliaIndexService indexService, 
            IAlgoliaSearchService<SearchResponse<Record>> searchService, 
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IUserService userService,
            IPublishedUrlProvider urlProvider,
            IContentService contentService,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory,
            IUmbracoContextFactory umbracoContextFactory, 
            ILogger<SearchController> logger, 
            IRecordBuilderFactory recordBuilderFactory,
            IAlgoliaGeolocationProvider algoliaGeolocationProvider)
        {
            _indexService = indexService;
            
            _searchService = searchService;

            _indexStorage = indexStorage;

            _userService = userService;

            _urlProvider = urlProvider;

            _contentService = contentService;

            _algoliaSearchPropertyIndexValueFactory = algoliaSearchPropertyIndexValueFactory;

            _umbracoContextFactory = umbracoContextFactory;
            
            _logger = logger;

            _recordBuilderFactory = recordBuilderFactory;

            _algoliaGeolocationProvider = algoliaGeolocationProvider;
        }

        [HttpGet]
        public IActionResult GetIndices()
        {
            
            var results = _indexStorage.Get().Select(p => new IndexConfiguration
            {
                Id = p.Id,
                Name = p.Name,
                ContentData = JsonSerializer.Deserialize<List<ContentData>>(p.SerializedData)
            });

            return new JsonResult(results);
        } 

        [HttpPost]
        public async Task<IActionResult> SaveIndex([FromBody] IndexConfiguration index)
        {
            try
            {
                _indexStorage.AddOrUpdate(new AlgoliaIndex
                {
                    Id = index.Id,
                    Name = index.Name,
                    SerializedData = JsonSerializer.Serialize(index.ContentData),
                    Date = DateTime.Now
                });

                var result = await _indexService.IndexExists(index.Name)
                    ? Result.Ok()
                    : await _indexService.PushData(index.Name);

                return new JsonResult(result);
            }
            catch(Exception ex)
            {
                return new JsonResult(Result.Fail(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> BuildIndex([FromBody] IndexConfiguration indexConfiguration)
        {
            try
            {
                var index = _indexStorage.GetById(indexConfiguration.Id);

                var payload = new List<Record>();

                var indexContentData = JsonSerializer.Deserialize<List<ContentData>>(index.SerializedData);

                foreach (var contentDataItem in indexContentData)
                {
                    using var ctx = _umbracoContextFactory.EnsureUmbracoContext();
                    var contentType = ctx.UmbracoContext.Content.GetContentType(contentDataItem.ContentType.Alias);
                    var contentItems = _contentService.GetPagedOfType(contentType.Id, 0, int.MaxValue, out _, null);

                    _logger.LogInformation("Building index for {ContentType} with {Count} items", contentDataItem.ContentType.Alias, contentItems.Count());

                    foreach (var contentItem in contentItems.Where(p => !p.Trashed && p.Published))
                    {
                        var record = new ContentRecordBuilder(
                                _userService, 
                                _urlProvider, 
                                _algoliaSearchPropertyIndexValueFactory, 
                                _recordBuilderFactory, 
                                _umbracoContextFactory,
                                _algoliaGeolocationProvider)
                            .BuildFromContent(contentItem, (p) => contentDataItem.Properties.Any(q => q.Alias == p.Alias))
                            .Build();

                        payload.Add(record);
                    }
                }

                var result = await _indexService.PushData(index.Name, payload);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(Result.Fail(ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteIndex(int id)
        {
            try
            {
                var indexName = _indexStorage.GetById(id).Name;

                _indexStorage.Delete(id);

                await _indexService.DeleteIndex(indexName);

                return new JsonResult(Result.Ok());
            }
            catch(Exception ex)
            {
                return new JsonResult(Result.Fail(ex.Message));
            }
        }

        [HttpGet]
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

            return new JsonResult(response);
        }
    }
}
