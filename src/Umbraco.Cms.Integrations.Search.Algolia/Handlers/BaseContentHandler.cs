using Microsoft.Extensions.Logging;

using System.Text.Json;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Integrations.Library.Services;
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

        protected readonly IPublishedUrlProvider _urlProvider;

        protected readonly IParserService _parserService;

        public BaseContentHandler(ILogger<BaseContentHandler> logger,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IAlgoliaIndexService indexService,
            IPublishedUrlProvider urlProvider,
            IParserService parserService)
        {
            Logger = logger;

            IndexStorage = indexStorage;

            IndexService = indexService;

            _urlProvider = urlProvider;    

            _parserService = parserService;
        }

        protected async Task RebuildIndex(IEnumerable<IContent> entities, bool deleteIndexData = false)
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

                        var record = new ContentRecordBuilder(_urlProvider, _parserService)
                           .BuildFromContent(entity, (p) => indexConfiguration.Properties.Any(q => q.Alias == p.Alias))
                           .Build();

                        var result = deleteIndexData
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
