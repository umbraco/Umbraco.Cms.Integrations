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
    public class ZapConfigService
    {
        private readonly IScopeProvider _scopeProvider;

#if NETCOREAPP
        private readonly ILogger<ZapConfigService> _logger;

        public ZapConfigService(IScopeProvider scopeProvider, ILogger<ZapConfigService> logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#else
        private readonly ILogger _logger;

         public ZapConfigService(IScopeProvider scopeProvider, ILogger logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#endif



        public IEnumerable<ContentConfigDto> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var data = scope.Database.Query<ZapContentConfigTable.ZapContentConfig>().ToList();

                return data.Select(p => new ContentConfigDto
                {
                    Id = p.Id,
                    ContentTypeName = p.ContentTypeName,
                    WebHookUrl = p.WebHookUrl
                });
            }
        }

        public string Add(ContentConfigDto contentConfigDto)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var zapContentConfig = new ZapContentConfigTable.ZapContentConfig
                    {
                        ContentTypeName = contentConfigDto.ContentTypeName,
                        WebHookUrl = contentConfigDto.WebHookUrl,
                        IsEnabled = true
                    };

                    scope.Database.Insert(zapContentConfig);
                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                var message = $"An error has occurred while adding a Zap config: {e.Message}";

#if NETCOREAPP
                _logger.LogError(message);
#else
                _logger.Error<ZapConfigService>(message);
#endif

                return message;
            }
        }

        public string Delete(int id)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database.Query<ZapContentConfigTable.ZapContentConfig>().FirstOrDefault(p => p.Id == id);
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
                var message = $"An error has occurred while deleting a Zap config: {e.Message}";

#if NETCOREAPP
                _logger.LogError(message);
#else
                _logger.Error<ZapConfigService>(message);
#endif

                return message;
            }
        }

        public string UpdatePreferences(string hookUrl, bool enabled)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database
                        .Query<ZapContentConfigTable.ZapContentConfig>().FirstOrDefault(p => p.WebHookUrl == hookUrl);
                    if (entity != null)
                    {
                        entity.IsEnabled = enabled;
                        scope.Database.Update(entity);
                    }

                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                var message = $"An error has occurred while updating a hook: {e.Message}";

#if NETCOREAPP
                _logger.LogError(message);
#else
                _logger.Error<ZapConfigService>(message);
#endif

                return message;
            }
        }

        public ContentConfigDto GetByName(string contentName)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entity =
                    scope.Database.FirstOrDefault<ZapContentConfigTable.ZapContentConfig>("where ContentTypeName = @0",
                        contentName);

                return entity != null ? new ContentConfigDto { WebHookUrl = entity.WebHookUrl } : null;
            }
        }
    }
}
