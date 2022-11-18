using Microsoft.Extensions.Logging;

using System.Text.Json;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class ContentUnpublishedHandler : BaseContentHandler, INotificationAsyncHandler<ContentUnpublishedNotification>
    {
        public ContentUnpublishedHandler(ILogger<ContentUnpublishedHandler> logger, IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage, IAlgoliaIndexService indexService)
           : base(logger, indexStorage, indexService)
        { }

        public async Task HandleAsync(ContentUnpublishedNotification notification, CancellationToken cancellationToken) =>
            await RebuildIndex(notification.UnpublishedEntities, true);
    }
}
