using Microsoft.Extensions.Logging;

using System.Text.Json;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class ContentPublishedHandler : INotificationAsyncHandler<ContentPublishedNotification>
    {
        private readonly ILogger<ContentPublishedHandler> _logger;

        private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;

        private readonly IAlgoliaIndexService _indexService;

        public ContentPublishedHandler(ILogger<ContentPublishedHandler> logger,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage, IAlgoliaIndexService algoliaIndexService)
        {
            _logger = logger;

            _indexStorage = indexStorage;

            _indexService = algoliaIndexService;
        }

        public async Task HandleAsync(ContentPublishedNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var indices = _indexStorage.Get();
                foreach (var publishedItem in notification.PublishedEntities)
                {
                    foreach (var index in indices)
                    {
                        var indexConfiguration = JsonSerializer.Deserialize<List<ContentData>>(index.SerializedData)
                            .FirstOrDefault(p => p.ContentType == publishedItem.ContentType.Alias);
                        if (indexConfiguration == null || indexConfiguration.ContentType != publishedItem.ContentType.Alias) continue;

                        var record = new RecordBuilder()
                            .BuildFromContent(publishedItem, (p) => indexConfiguration.Properties.Any(q => q == p.Alias))
                            .Build();

                        var result = await _indexService.UpdateData(index.Name, record);

                        if (!string.IsNullOrEmpty(result))
                            _logger.LogError($"Failed to update data for Algolia index: {result}");
                    }

                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to update data for Algolia index: {ex.Message}");
            }
        }
    }
}
