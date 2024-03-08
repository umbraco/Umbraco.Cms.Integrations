using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using System.Text.Json;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;

namespace Umbraco.Cms.Integrations.Search.Algolia.Handlers
{
    public class AlgoliaContentCacheRefresherHandler : INotificationAsyncHandler<ContentCacheRefresherNotification>
    {
        private readonly IServerRoleAccessor _serverRoleAccessor;

        private readonly IContentService _contentService;

        private readonly ILogger _logger;

        private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;

        private readonly IAlgoliaIndexService _indexService;

        private readonly IUserService _userService;

        private readonly IPublishedUrlProvider _urlProvider;

        private readonly IAlgoliaSearchPropertyIndexValueFactory _algoliaSearchPropertyIndexValueFactory;

        private readonly IRecordBuilderFactory _recordBuilderFactory;

        public AlgoliaContentCacheRefresherHandler(
            IServerRoleAccessor serverRoleAccessor,
            ILogger<AlgoliaContentCacheRefresherHandler> logger,
            IContentService contentService,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IAlgoliaIndexService indexService,
            IUserService userService,
            IPublishedUrlProvider urlProvider,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory, 
            IRecordBuilderFactory recordBuilderFactory)
        {
            _serverRoleAccessor = serverRoleAccessor;
            _contentService = contentService;
            _logger = logger;   
            _indexStorage = indexStorage;
            _indexService = indexService;
            _userService = userService;
            _urlProvider = urlProvider;
            _algoliaSearchPropertyIndexValueFactory = algoliaSearchPropertyIndexValueFactory;
            _recordBuilderFactory = recordBuilderFactory;
        }

        public async Task HandleAsync(ContentCacheRefresherNotification notification, CancellationToken cancellationToken)
        {
            if (notification.MessageObject is not ContentCacheRefresher.JsonPayload[] payloads)
            {
                return;
            }

            switch (_serverRoleAccessor.CurrentServerRole)
            {
                case ServerRole.Subscriber:
                    _logger.LogDebug("Algolia indexing task will not run on subscriber servers.");
                    return;
                case ServerRole.Unknown:
                    _logger.LogDebug("Algolia indexing task will not run on servers with unknown role.");
                    return;
                case ServerRole.Single:
                case ServerRole.SchedulingPublisher:
                default:
                    break;
            }

            var refreshedContent = _contentService
                .GetByIds(
                    payloads
                        .Where(p => p.ChangeTypes == TreeChangeTypes.RefreshNode || p.ChangeTypes == TreeChangeTypes.RefreshBranch)
                        .Select(p => p.Id));

            await RebuildIndex(refreshedContent);
        }

        protected async Task RebuildIndex(IEnumerable<IContent> entities)
        {
            try
            {
                var indices = _indexStorage.Get();

                foreach (var entity in entities)
                {
                    foreach (var index in indices)
                    {
                        var indexConfiguration = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData)
                            .FirstOrDefault(p => p.Alias == entity.ContentType.Alias);
                        if (indexConfiguration == null || indexConfiguration.Alias != entity.ContentType.Alias) continue;

                        var record = new ContentRecordBuilder(_userService, _urlProvider, _algoliaSearchPropertyIndexValueFactory, _recordBuilderFactory)
                           .BuildFromContent(entity, (p) => indexConfiguration.Properties.Any(q => q.Alias == p.Alias))
                           .Build();

                        var result = entity.Trashed || !entity.Published
                         ? await _indexService.DeleteData(index.Name, entity.Key.ToString())
                         : await _indexService.UpdateData(index.Name, record);

                        if (result.Failure)
                            _logger.LogError($"Failed to update data for Algolia index: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update data for Algolia index: {ex.Message}");
            }
        }
    }
}
