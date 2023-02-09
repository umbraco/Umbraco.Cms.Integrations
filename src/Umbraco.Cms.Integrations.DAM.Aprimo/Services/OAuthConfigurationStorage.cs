using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Integrations.DAM.Aprimo.Migrations;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public class OAuthConfigurationStorage
    {
        private readonly IScopeProvider _scopeProvider;

        public OAuthConfigurationStorage(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public void AddOrUpdate(AprimoOAuthConfiguration entity)
        {
            using var scope = _scopeProvider.CreateScope();

            if (entity.Id == 0)
                scope.Database.Insert(entity);
            else
                scope.Database.Update(entity);

            scope.Complete();
        }

        public AprimoOAuthConfiguration Get()
        {
            using var scope = _scopeProvider.CreateScope();

            return scope.Database.FirstOrDefault<AprimoOAuthConfiguration>(string.Empty);
        }

        public void Delete(int id)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = scope.Database.SingleById<AprimoOAuthConfiguration>(id);

            scope.Database.Delete(entity);

            scope.Complete();
        }
    }
}
