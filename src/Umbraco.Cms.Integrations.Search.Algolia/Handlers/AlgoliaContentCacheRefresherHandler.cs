using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Services;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class AlgoliaContentCacheRefresherHandler : BaseContentHandler, INotificationAsyncHandler<ContentCacheRefresherNotification>
    {
        private readonly IContentService _contentService;

        public AlgoliaContentCacheRefresherHandler(
            ILogger<AlgoliaContentCacheRefresherHandler> logger,
            IContentService contentService,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IAlgoliaIndexService indexService,
            IUserService userService,
            IPublishedUrlProvider urlProvider,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory)
            : base(logger, indexStorage, indexService, userService, urlProvider, algoliaSearchPropertyIndexValueFactory)
        {
            _contentService = contentService;
        }

        public async Task HandleAsync(ContentCacheRefresherNotification notification, CancellationToken cancellationToken)
        {
            if (notification.MessageObject is not ContentCacheRefresher.JsonPayload[] payloads)
            {
                return;
            }

            var refreshedContent = _contentService
                .GetByIds(
                    payloads
                        .Where(p => p.ChangeTypes == TreeChangeTypes.RefreshNode || p.ChangeTypes == TreeChangeTypes.RefreshBranch)
                        .Select(p => p.Id));

            await RebuildIndex(refreshedContent);
        }

        public void Handle(ContentCacheRefresherNotification notification)
        {
            if (notification.MessageObject is not ContentCacheRefresher.JsonPayload[] payloads)
            {
                return;
            }

            foreach (ContentCacheRefresher.JsonPayload payload in payloads)
            {
                if (payload.ChangeTypes != TreeChangeTypes.RefreshNode 
                    && payload.ChangeTypes != TreeChangeTypes.RefreshBranch)
                {
                    return;
                }

                var x = _contentService.GetById(1205);
                var y = x.Published;

                // You can do stuff with the ID of the refreshed content, for instance getting it from the content service.
                var refeshedContent = _contentService.GetById(payload.Id);
            }
        }
    }
}
