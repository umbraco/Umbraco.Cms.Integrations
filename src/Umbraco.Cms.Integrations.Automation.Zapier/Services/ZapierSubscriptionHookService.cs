using System;
using System.Collections.Generic;
using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;

#if NETCOREAPP
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Core.Logging;
using Umbraco.Core.Scoping;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    /// <summary>
    /// Subscription service handling database operations for content nodes.
    /// </summary>
    public class ZapierSubscriptionHookService
    {
        private readonly IScopeProvider _scopeProvider;

#if NETCOREAPP
        private readonly ILogger<ZapierSubscriptionHookService> _logger;

        public ZapierSubscriptionHookService(IScopeProvider scopeProvider, ILogger<ZapierSubscriptionHookService> logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#else
        private readonly ILogger _logger;

         public ZapierSubscriptionHookService(IScopeProvider scopeProvider, ILogger logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#endif

        public IEnumerable<SubscriptionDto> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var data = scope.Database.Query<ZapierMigration.ZapierSubscriptionHookTable>().ToList();

                return data.Select(p => new SubscriptionDto
                {
                    Id = p.Id,
                    EntityId = p.EntityId,
                    HookUrl = p.HookUrl
                });
            }
        }

        public string Add(string entityId, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var zapContentConfig = new ZapierMigration.ZapierSubscriptionHookTable
                    {
                        EntityId = entityId,
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

#if NETCOREAPP
                _logger.LogError(message);
#else
                _logger.Error<ZapierSubscriptionHookService>(message);
#endif

                return message;
            }
        }

        public string Delete(string entityId, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database.Query<ZapierMigration.ZapierSubscriptionHookTable>()
                        .FirstOrDefault(p => p.EntityId == entityId && p.HookUrl == hookUrl);
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

#if NETCOREAPP
                _logger.LogError(message);
#else
                _logger.Error<ZapierSubscriptionHookService>(message);
#endif

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

                return entities.Any();
            }
        }
    }
}
