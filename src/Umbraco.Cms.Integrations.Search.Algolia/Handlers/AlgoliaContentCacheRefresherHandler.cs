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
using Umbraco.Extensions;

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

        private readonly IUmbracoContextFactory _umbracoContextFactory;

        private readonly IAlgoliaGeolocationProvider _algoliaGeolocationProvider;

        public AlgoliaContentCacheRefresherHandler(
            IServerRoleAccessor serverRoleAccessor,
            ILogger<AlgoliaContentCacheRefresherHandler> logger,
            IContentService contentService,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IAlgoliaIndexService indexService,
            IUserService userService,
            IPublishedUrlProvider urlProvider,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory, 
            IRecordBuilderFactory recordBuilderFactory,
            IUmbracoContextFactory umbracoContextFactory,
            IAlgoliaGeolocationProvider algoliaGeolocationProvider)
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
            _umbracoContextFactory = umbracoContextFactory;
            _algoliaGeolocationProvider = algoliaGeolocationProvider;
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

            var refreshedContent = new List<IContent>();

            foreach (var payload in payloads)
            {
                var isBranchChange = payload.ChangeTypes.HasType(TreeChangeTypes.RefreshBranch);

                if (!isBranchChange && !payload.ChangeTypes.HasType(TreeChangeTypes.RefreshNode)) continue;

                var content = _contentService.GetById(payload.Id);
                if (content == null) continue;

                refreshedContent.Add(content);

                // A branch change - moving a node to the recycle bin, for example - only sends a payload for the
                // branch root, so its descendants have to be collected here or they are left stale in the index.
                if (isBranchChange)
                {
                    refreshedContent.AddRange(GetDescendants(content));
                }
            }

            await RebuildIndex(refreshedContent);
        }

        private IEnumerable<IContent> GetDescendants(IContent content)
        {
            const int pageSize = 500;

            var pageIndex = 0;
            long total;

            do
            {
                var page = _contentService.GetPagedDescendants(content.Id, pageIndex, pageSize, out total);

                foreach (var descendant in page)
                {
                    yield return descendant;
                }

                pageIndex++;
            }
            while (pageIndex * pageSize < total);
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

                        var record = new ContentRecordBuilder(
                                _userService, 
                                _urlProvider, 
                                _algoliaSearchPropertyIndexValueFactory, 
                                _recordBuilderFactory, 
                                _umbracoContextFactory,
                                _algoliaGeolocationProvider)
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
