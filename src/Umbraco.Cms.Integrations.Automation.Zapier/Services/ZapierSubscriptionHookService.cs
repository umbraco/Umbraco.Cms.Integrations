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



        public IEnumerable<ContentConfigDto> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var data = scope.Database.Query<ZapierSubscriptionHookTable.ZapierSubscriptionHook>().ToList();

                return data.Select(p => new ContentConfigDto
                {
                    Id = p.Id,
                    ContentTypeAlias = p.ContentTypeAlias,
                    HookUrl = p.HookUrl
                });
            }
        }

        public string Add(string contentTypeAlias, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var zapContentConfig = new ZapierSubscriptionHookTable.ZapierSubscriptionHook
                    {
                        ContentTypeAlias = contentTypeAlias,
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

        public string Delete(string contentTypeAlias, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database.Query<ZapierSubscriptionHookTable.ZapierSubscriptionHook>()
                        .FirstOrDefault(p => p.ContentTypeAlias == contentTypeAlias && p.HookUrl == hookUrl);
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

        public bool TryGetByAlias(string contentTypeAlias, out ContentConfigDto dto)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entity =
                    scope.Database
                        .FirstOrDefault<ZapierSubscriptionHookTable.ZapierSubscriptionHook>("where ContentTypeAlias = @0", contentTypeAlias);

                dto = entity != null
                    ? new ContentConfigDto {HookUrl = entity.HookUrl}
                    : null;

                return entity != null;
            }
        }
    }
}
