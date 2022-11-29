
namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaIndexDefinitionStorage<T>
        where T : class
    {
        List<T> Get();

        T GetById(int id);

        void AddOrUpdate(T entity);

        void Delete(int id);
    }
}
