using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class ContentUnpublishedHandler : BaseContentHandler, INotificationAsyncHandler<ContentUnpublishedNotification>
    {
        public ContentUnpublishedHandler(
            ILogger<ContentUnpublishedHandler> logger,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IAlgoliaIndexService indexService,
            IPublishedUrlProvider urlProvider,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory)
           : base(logger, indexStorage, indexService, urlProvider, algoliaSearchPropertyIndexValueFactory)
        { }

        public async Task HandleAsync(ContentUnpublishedNotification notification, CancellationToken cancellationToken) =>
            await RebuildIndex(notification.UnpublishedEntities, true);
    }
}
