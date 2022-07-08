
namespace Umbraco.Integrations.Library.Services
{
    public interface ICacheHelper
    {
        bool TryGetCachedItem<T>(string key, out T item) where T : class;

        void AddCachedItem(string key, string serializedItem);

        void ClearCachedItems();
    }
}
