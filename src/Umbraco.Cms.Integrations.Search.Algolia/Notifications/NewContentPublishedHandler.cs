using Microsoft.Extensions.Logging;

using System.Text.Json;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Notifications
{
    public class NewContentPublishedHandler : INotificationHandler<ContentPublishedNotification>
    {
        private readonly ILogger<NewContentPublishedHandler> _logger;

        private readonly IScopeService<AlgoliaIndex> _scopeService;

        private readonly IAlgoliaIndexService _indexService;

        public NewContentPublishedHandler(ILogger<NewContentPublishedHandler> logger,
            IScopeService<AlgoliaIndex> scopeService, IAlgoliaIndexService algoliaIndexService)
        {
            _logger = logger;

            _scopeService = scopeService;

            _indexService = algoliaIndexService;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            try
            {
                foreach (var publishedItem in notification.PublishedEntities)
                {
                    var indices = _scopeService.GetByContentTypeAlias(publishedItem.ContentType.Alias);

                    foreach (var index in indices)
                    {
                        var indexConfiguration = JsonSerializer.Deserialize<List<ContentData>>(index.SerializedData)
                            .FirstOrDefault(p => p.ContentType == publishedItem.ContentType.Alias);
                        if (indexConfiguration == null) continue;

                        var record = new RecordBuilder()
                            .BuildFromContent(publishedItem, (p) => indexConfiguration.Properties.Any(q => q == p.Alias))
                            .Build();

                        var result = _indexService.UpdateData(index.Name, record).ConfigureAwait(false).GetAwaiter().GetResult();

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
