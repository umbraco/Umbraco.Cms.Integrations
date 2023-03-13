using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Services;
using Umbraco.Cms.Web.Common.UmbracoContext;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class ContentDeletedHandler : BaseContentHandler, INotificationAsyncHandler<ContentDeletedNotification>
    {
        public ContentDeletedHandler(
            ILogger<ContentDeletedHandler> logger, 
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage, 
            IAlgoliaIndexService indexService,
            IPublishedUrlProvider urlProvider,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory)
           : base(logger, indexStorage, indexService, urlProvider, algoliaSearchPropertyIndexValueFactory)
        { }

        public async Task HandleAsync(ContentDeletedNotification notification, CancellationToken cancellationToken) =>
            await RebuildIndex(notification.DeletedEntities, true);
    }
}
