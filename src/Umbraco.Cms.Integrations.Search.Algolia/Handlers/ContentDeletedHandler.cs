using Microsoft.Extensions.Logging;

using System.Text.Json;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class ContentDeletedHandler : INotificationAsyncHandler<ContentDeletedNotification>
    {
        private readonly ILogger<ContentDeletedNotification> _logger;

        private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _scopeService;

        private readonly IAlgoliaIndexService _indexService;

        public ContentDeletedHandler(ILogger<ContentDeletedNotification> logger, IAlgoliaIndexDefinitionStorage<AlgoliaIndex> scopeService, IAlgoliaIndexService indexService)
        {
            _logger = logger;

            _scopeService = scopeService;

            _indexService = indexService;
        }

        public async Task HandleAsync(ContentDeletedNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var indices = _scopeService.Get();

                foreach (var publishedItem in notification.DeletedEntities)
                {
                    foreach (var index in indices)
                    {
                        var indexConfiguration = JsonSerializer.Deserialize<List<ContentData>>(index.SerializedData)
                            .FirstOrDefault(p => p.ContentType == publishedItem.ContentType.Alias);
                        if (indexConfiguration == null || indexConfiguration.ContentType != publishedItem.ContentType.Alias) continue;

                        var result = await _indexService.DeleteData(index.Name, publishedItem.Key.ToString());

                        if (!string.IsNullOrEmpty(result))
                            _logger.LogError($"Failed to delete data for Algolia index: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete data for Algolia index: {ex.Message}");
            }
        }
    }
}
