
namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaSearchService<T>
    {
        T Search(string indexName, string query);
    }
}
