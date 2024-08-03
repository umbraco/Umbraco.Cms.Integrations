using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    /// <summary>
    /// Subscription service handling database operations for content nodes.
    /// </summary>
    public class ZapierSubscriptionHookService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<ZapierSubscriptionHookService> _logger;

        public ZapierSubscriptionHookService(IScopeProvider scopeProvider, ILogger<ZapierSubscriptionHookService> logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }

        public IEnumerable<SubscriptionDto> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var data = scope.Database.Query<ZapierMigration.ZapierSubscriptionHookTable>().ToList();

                return data.Select(p => new SubscriptionDto
                {
                    Id = p.Id,
                    EntityId = p.EntityId,
                    Type = p.Type,
                    HookUrl = p.HookUrl
                });
            }
        }

        public string Add(string entityId, int type, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var zapContentConfig = new ZapierMigration.ZapierSubscriptionHookTable
                    {
                        EntityId = entityId,
                        Type = type,
                        HookUrl = hookUrl,
                    };

                    scope.Database.Insert(zapContentConfig);
                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                var message = $"An error has occurred while adding a subscription hook: {e.Message}";

                _logger.LogError(message);

                return message;
            }
        }

        public string Delete(string entityId, int type, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database.Query<ZapierMigration.ZapierSubscriptionHookTable>()
                        .FirstOrDefault(p => p.EntityId == entityId && p.Type == type && p.HookUrl == hookUrl);
                    if (entity != null)
                    {
                        scope.Database.Delete(entity);
                    }
                    
                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                var message = $"An error has occurred while deleting a subscription hook: {e.Message}";

                _logger.LogError(message);

                return message;
            }
        }

        public bool TryGetByAlias(string contentTypeAlias, out IEnumerable<SubscriptionDto> dto)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entities =
                    scope.Database
                        .Query<ZapierMigration.ZapierSubscriptionHookTable>()
                        .Where(p => p.EntityId == contentTypeAlias)
                        .ToList();

                dto = entities.Any()
                    ? entities.Select(p => new SubscriptionDto { HookUrl = p.HookUrl })
                    : null;

                scope.Complete();
                return entities.Any();
            }
        }
    }
}
