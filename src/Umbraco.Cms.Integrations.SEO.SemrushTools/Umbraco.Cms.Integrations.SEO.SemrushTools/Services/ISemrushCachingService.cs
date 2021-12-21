
namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Services
{
    public interface ISemrushCachingService<T>
    {
        bool TryGetCachedItem(out T item, string key);

        void AddCachedItem(string key, string serializedItem);
    }
}
