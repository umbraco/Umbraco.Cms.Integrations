using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class ContentPublishedHandler : BaseContentHandler, INotificationAsyncHandler<ContentPublishedNotification>
    {
        public ContentPublishedHandler(ILogger<ContentPublishedHandler> logger, IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage, IAlgoliaIndexService indexService)
            :  base(logger, indexStorage, indexService)
        { }

        public async Task HandleAsync(ContentPublishedNotification notification, CancellationToken cancellationToken) => await RebuildIndex(notification.PublishedEntities);
    }
}
