#if NETCOREAPP
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;
#else
using Umbraco.Core.Logging;
using Umbraco.Core.Scoping;
#endif
using System;
using Umbraco.Cms.Integrations.Crm.Dynamics.Migrations;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class DynamicsConfigurationService
    {
        private readonly IScopeProvider _scopeProvider;

#if NETCOREAPP
        private readonly ILogger<DynamicsService> _logger;

        public DynamicsConfigurationService(IScopeProvider scopeProvider, ILogger<DynamicsService> logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#else
        private readonly ILogger _logger;

        public DynamicsConfigurationService(IScopeProvider scopeProvider, ILogger logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }
#endif

        public string AddorUpdateOAuthConfiguration(string accessToken, string userId, string fullName)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database
                        .Query<DynamicsOAuthConfigurationTable>()
                        .FirstOrDefault(p => p.UserId == userId) ?? new DynamicsOAuthConfigurationTable { UserId = userId, FullName = fullName };

                    entity.AccessToken = accessToken;

                    if (entity.Id == 0)
                        scope.Database.Insert(entity);
                    else
                        scope.Database.Update(entity);

                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                var message = $"An error has occurred while adding the OAuth configuration: {ex.Message}";

#if NETCOREAPP
                _logger.LogError(message);
#else
                _logger.Error<DynamicsService>(message);
#endif
                return message;
            }
        }

        public OAuthConfigurationDto GetOAuthConfiguration()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entity = scope.Database
                    .Query<DynamicsOAuthConfigurationTable>()
                    .FirstOrDefault();

                return entity != null
                    ? new OAuthConfigurationDto { Id = entity.Id, AccessToken = entity.AccessToken, UserId = entity.UserId, FullName = entity.FullName } 
                    : null;
            }
        }

        public string GetSystemUserFullName()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entity = scope.Database
                    .Query<DynamicsOAuthConfigurationTable>()
                    .FirstOrDefault();

                return entity != null ? entity.FullName : string.Empty;
            }
        }

        public string Delete()
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database
                        .Query<DynamicsOAuthConfigurationTable>()
                        .FirstOrDefault();
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
                var message = $"An error has occurred while deleting the OAuth configuration: {e.Message}";

#if NETCOREAPP
                _logger.LogError(message);
#else
                _logger.Error<DynamicsConfigurationService>(message);
#endif

                return message;
            }
        }

    }
}
