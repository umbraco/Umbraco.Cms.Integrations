using System;

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos;
using Umbraco.Core.Cache;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Services
{
    public class SemrushCachingService: ISemrushCachingService<RelatedPhrasesDto>
    {
        private readonly IAppPolicyCache _cache;

        public SemrushCachingService(AppCaches appCaches)
        {
            _cache = appCaches.RuntimeCache;
        }

        public bool TryGetCachedItem(out RelatedPhrasesDto item, string key)
        {
            var serializedItem = _cache.GetCacheItem<string>(key);

            item = string.IsNullOrEmpty(serializedItem)
                ? new RelatedPhrasesDto()
                : JsonConvert.DeserializeObject<RelatedPhrasesDto>(serializedItem);

            return !string.IsNullOrEmpty(serializedItem);
        }

        public void AddCachedItem(string key, string item)
        {
            _cache.InsertCacheItem(key, () => item, TimeSpan.FromHours(1));
        }
    }
}
