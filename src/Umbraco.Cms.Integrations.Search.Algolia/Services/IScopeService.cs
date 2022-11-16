
namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IScopeService<T>
        where T : class
    {
        List<T> Get();

        T GetById(int id);

        List<T> GetByContentTypeAlias(string alias);

        void AddOrUpdate(T entity);

        void Delete(int id);
    }
}
