using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class ScopeService : IScopeService<AlgoliaIndex>
    {
        private readonly IScopeProvider _scopeProvider;

        public ScopeService(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public void AddOrUpdate(AlgoliaIndex entity)
        {
            using var scope = _scopeProvider.CreateScope();

            if (entity.Id == 0)
                scope.Database.Insert(entity);
            else
                scope.Database.Update(entity);

            scope.Complete();
        }

        public List<AlgoliaIndex> Get()
        {
            using var scope = _scopeProvider.CreateScope();

            return scope.Database.Fetch<AlgoliaIndex>();
        }


        public AlgoliaIndex GetById(int id)
        {
            using var scope = _scopeProvider.CreateScope();

            return scope.Database.Single<AlgoliaIndex>("where Id = " + id);
        }

        public List<AlgoliaIndex> GetByContentTypeAlias(string alias)
        {
            using var scope = _scopeProvider.CreateScope();

            return scope.Database.Fetch<AlgoliaIndex>("where SerializedData like '%" + alias + "%'");
        }

        public void Delete(int id)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = scope.Database.SingleById<AlgoliaIndex>(id);

            scope.Database.Delete(entity);

            scope.Complete();
        }

       
    }
}
