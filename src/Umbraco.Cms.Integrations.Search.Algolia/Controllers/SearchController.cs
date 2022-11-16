using Algolia.Search.Models.Search;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
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

        private readonly IScopeService<AlgoliaIndex> _scopeService;

        private readonly UmbracoHelper _umbracoHelper;

        public SearchController(IAlgoliaIndexService indexService, IAlgoliaSearchService<SearchResponse<Record>> searchService, 
            IScopeService<AlgoliaIndex> scopeService,
            UmbracoHelper umbracoHelper)
        {
            _indexService = indexService;
            
            _searchService = searchService;

            _scopeService = scopeService;

            _umbracoHelper = umbracoHelper;
        }

        [HttpGet]
        public IActionResult GetIndices()
        {
            var results = _scopeService.Get().Select(p => new IndexConfiguration
            {
                Id = p.Id,
                Name = p.Name,
                ContentData = JsonSerializer.Deserialize<List<ContentData>>(p.SerializedData)
            });

            return new JsonResult(results);
        } 

        [HttpPost]
        public string SaveIndex([FromBody] IndexConfiguration index)
        {
            try
            {
                _scopeService.AddOrUpdate(new AlgoliaIndex
                {
                    Id = index.Id,
                    Name = index.Name,
                    SerializedData = JsonSerializer.Serialize(index.ContentData),
                    Date = DateTime.Now
                });

                var payload = new List<Record>();

                foreach (var item in index.ContentData)
                {
                    var contentItems = _umbracoHelper.ContentAtXPath($"//{item.ContentType}");

                    foreach (var contentItem in contentItems)
                    {
                        var record = new RecordBuilder()
                            .BuildFromContent(contentItem, (p) => item.Properties.Any(q => q == p.Alias))
                            .Build();

                        payload.Add(record);
                    }
                }

                return _indexService.PushData(index.Name, payload);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpDelete]
        public string DeleteIndex(int id)
        {
            try
            {
                _scopeService.Delete(id);

                return string.Empty;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public IActionResult Search(int indexId, string query)
        {
            var index = _scopeService.GetById(indexId);

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
