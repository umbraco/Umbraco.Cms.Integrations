
namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public interface ICacheHelper
    {
        bool TryGetCachedItem<T>(string key, out T item) where T : class;

        void AddCachedItem(string key, string serializedItem);

        void ClearCachedItems();
    }
}
