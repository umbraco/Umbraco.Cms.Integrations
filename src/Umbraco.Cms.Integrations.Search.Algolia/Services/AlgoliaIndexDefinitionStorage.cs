using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaIndexDefinitionStorage : IAlgoliaIndexDefinitionStorage<AlgoliaIndex>
    {
        private readonly IScopeProvider _scopeProvider;

        public AlgoliaIndexDefinitionStorage(IScopeProvider scopeProvider)
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

        public void Delete(int id)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = scope.Database.SingleById<AlgoliaIndex>(id);

            scope.Database.Delete(entity);

            scope.Complete();
        }

       
    }
}
