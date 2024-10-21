using System.Text.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public class CacheHelper: ICacheHelper
    {
        private readonly IAppPolicyCache _cache;

        public CacheHelper(AppCaches appCaches)
        {
            _cache = appCaches.RuntimeCache;
        }

        public bool TryGetCachedItem<T>(string key, out T item) where T : class
        {
            var serializedItem = _cache.GetCacheItem<string>(key);

            item = string.IsNullOrEmpty(serializedItem)
                ? null
                : JsonSerializer.Deserialize<T>(serializedItem);

            return !string.IsNullOrEmpty(serializedItem);
        }

        public void AddCachedItem(string key, string item)
        {
            _cache.InsertCacheItem(key, () => item, TimeSpan.FromHours(1));
        }

        public void ClearCachedItems()
        {
            _cache.Clear();
        }
    }
}
