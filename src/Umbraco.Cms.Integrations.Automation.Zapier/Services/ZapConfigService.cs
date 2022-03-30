using System;
using System.Collections.Generic;
using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Core.Logging;
using Umbraco.Core.Scoping;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapConfigService
    {
        private readonly IScopeProvider _scopeProvider;

        private readonly ILogger _logger;

        public ZapConfigService(IScopeProvider scopeProvider, ILogger logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }

        public IEnumerable<ContentConfigDto> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var data = scope.Database.Query<ZapContentConfigTable.ZapContentConfig>().ToList();

                return data.Select(p => new ContentConfigDto
                {
                    Id = p.Id,
                    ContentTypeAlias = p.ContentTypeAlias,
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
                        ContentTypeAlias = contentConfigDto.ContentTypeAlias,
                        WebHookUrl = contentConfigDto.WebHookUrl
                    };

                    scope.Database.Insert(zapContentConfig);
                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                var message = $"An error has occurred while adding a Zap config: {e.Message}";

                _logger.Error<ZapConfigService>(message);

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

                _logger.Error<ZapConfigService>(message);

                return message;
            }
        }

        public ContentConfigDto GetByAlias(string contentAlias)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entity =
                    scope.Database.FirstOrDefault<ZapContentConfigTable.ZapContentConfig>("where ContentTypeAlias = @0",
                        contentAlias);

                return entity != null ? new ContentConfigDto(entity.WebHookUrl) : null;
            }
        }
    }
}
