﻿using Algolia.Search.Models.Search;
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
    public class BuildIndexController : SearchControllerBase
    {
        public BuildIndexController(
            IAlgoliaIndexService indexService,
            IAlgoliaSearchService<SearchResponse<Record>> searchService,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IUserService userService, IPublishedUrlProvider urlProvider,
            IContentService contentService,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory,
            IUmbracoContextFactory umbracoContextFactory,
            ILogger<BuildIndexController> logger,
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

        [HttpPost("index/build")]
        public async Task<IActionResult> BuildIndex([FromBody] IndexConfiguration indexConfiguration)
        {
            var index = IndexStorage.GetById(indexConfiguration.Id);

            var payload = new List<Record>();

            var indexContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData);

            foreach (var contentDataItem in indexContentData)
            {
                using var ctx = UmbracoContextFactory.EnsureUmbracoContext();
                var contentType = ctx.UmbracoContext.Content.GetContentType(contentDataItem.Alias);
                var contentItems = ContentService.GetPagedOfType(contentType.Id, 0, int.MaxValue, out _, null);

                Logger.LogInformation("Building index for {ContentType} with {Count} items", contentDataItem.Alias, contentItems.Count());

                foreach (var contentItem in contentItems.Where(p => !p.Trashed))
                {
                    var record = new ContentRecordBuilder(UserService, UrlProvider, AlgoliaSearchPropertyIndexValueFactory, RecordBuilderFactory, UmbracoContextFactory)
                        .BuildFromContent(contentItem, (p) => contentDataItem.Properties.Any(q => q.Alias == p.Alias))
                        .Build();

                    payload.Add(record);
                }
            }

            var result = await IndexService.PushData(index.Name, payload);

            return Ok(result);
        }
    }
}
