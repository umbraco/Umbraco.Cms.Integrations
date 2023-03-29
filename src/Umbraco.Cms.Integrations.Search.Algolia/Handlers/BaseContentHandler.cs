using Microsoft.Extensions.Logging;

using System.Text.Json;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public abstract class BaseContentHandler
    {
        protected readonly ILogger Logger;

        protected readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> IndexStorage;

        protected readonly IAlgoliaIndexService IndexService;

        protected readonly IUserService UserService;

        protected readonly IPublishedUrlProvider UrlProvider;

        protected readonly IAlgoliaSearchPropertyIndexValueFactory AlgoliaSearchPropertyIndexValueFactory;

        public BaseContentHandler(ILogger<BaseContentHandler> logger,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IAlgoliaIndexService indexService,
            IUserService userService,
            IPublishedUrlProvider urlProvider,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory)
        {
            Logger = logger;

            IndexStorage = indexStorage;

            IndexService = indexService;

            UserService = userService;

            UrlProvider = urlProvider;

            AlgoliaSearchPropertyIndexValueFactory = algoliaSearchPropertyIndexValueFactory;
        }

        protected async Task RebuildIndex(IEnumerable<IContent> entities)
        {
            try
            {
                var indices = IndexStorage.Get();

                foreach (var entity in entities)
                {
                    foreach (var index in indices)
                    {
                        var indexConfiguration = JsonSerializer.Deserialize<List<ContentData>>(index.SerializedData)
                            .FirstOrDefault(p => p.ContentType.Alias == entity.ContentType.Alias);
                        if (indexConfiguration == null || indexConfiguration.ContentType.Alias != entity.ContentType.Alias) continue;

                        var record = new ContentRecordBuilder(UserService, UrlProvider, AlgoliaSearchPropertyIndexValueFactory)
                           .BuildFromContent(entity, (p) => indexConfiguration.Properties.Any(q => q.Alias == p.Alias))
                           .Build();

                        var result = entity.Trashed || !entity.Published
                         ? await IndexService.DeleteData(index.Name, entity.Key.ToString())
                         : await IndexService.UpdateData(index.Name, record);

                        if (result.Failure)
                            Logger.LogError($"Failed to update data for Algolia index: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to update data for Algolia index: {ex.Message}");
            }
        }
    }
}
