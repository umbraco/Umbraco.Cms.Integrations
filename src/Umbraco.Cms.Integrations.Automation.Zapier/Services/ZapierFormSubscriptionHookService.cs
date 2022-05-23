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
    /// Subscription service handling database operations for Forms.
    /// </summary>
    public class ZapierFormSubscriptionHookService
    {
        private readonly IScopeProvider _scopeProvider;

#if NETCOREAPP
        private readonly ILogger<ZapierSubscriptionHookService> _logger;

        public ZapierFormSubscriptionHookService(IScopeProvider scopeProvider, ILogger<ZapierSubscriptionHookService> logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#else
        private readonly ILogger _logger;

         public ZapierFormSubscriptionHookService(IScopeProvider scopeProvider, ILogger logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#endif

        public IEnumerable<FormConfigDto> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var data = scope.Database.Query<ZapierMigration.ZapierFormSubscriptionHookTable>().ToList();

                return data.Select(p => new FormConfigDto
                {
                    Id = p.Id,
                    FormName = p.FormName,
                    HookUrl = p.HookUrl
                });
            }
        }

        public string Add(string formName, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var zapContentConfig = new ZapierMigration.ZapierFormSubscriptionHookTable()
                    {
                        FormName = formName,
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
                _logger.Error<ZapierFormSubscriptionHookService>(message);
#endif

                return message;
            }
        }

        public string Delete(string formName, string hookUrl)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database.Query<ZapierMigration.ZapierFormSubscriptionHookTable>()
                        .FirstOrDefault(p => p.FormName == formName && p.HookUrl == hookUrl);
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
                _logger.Error<ZapierFormSubscriptionHookService>(message);
#endif

                return message;
            }
        }
    }
}
