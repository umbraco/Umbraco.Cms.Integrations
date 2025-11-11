using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Sync;
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
            if (_serverRoleAccessor.CurrentServerRole == ServerRole.SchedulingPublisher)
            {
                _distributedCache.RefreshAllPublishedSnapshot();
            }
            return Task.CompletedTask;
        }
    }
}
