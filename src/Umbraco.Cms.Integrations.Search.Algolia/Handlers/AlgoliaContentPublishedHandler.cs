using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Sync;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class AlgoliaContentPublishedHandler : INotificationAsyncHandler<ContentPublishedNotification>
    {
        private readonly IServerRoleAccessor _serverRoleAccessor;
        private readonly DistributedCache _distributedCache;

        public AlgoliaContentPublishedHandler(
            IServerRoleAccessor serverRoleAccessor,
            DistributedCache distributedCache)
        {
            _serverRoleAccessor = serverRoleAccessor;
            _distributedCache = distributedCache;
        }

        public Task HandleAsync(ContentPublishedNotification notification, CancellationToken cancellationToken)
        {
            if (_serverRoleAccessor.CurrentServerRole != ServerRole.SchedulingPublisher)
            {
                return Task.CompletedTask;
            }

            var changes = notification.PublishedEntities
                .Select(entity => new TreeChange<IContent>(entity, TreeChangeTypes.RefreshNode));

#if NET8_0_OR_GREATER
            _distributedCache.RefreshContentCache(changes);
#else
            _distributedCache.RefreshContentCache(changes.ToArray());
#endif

            return Task.CompletedTask;
        }
    }
}
