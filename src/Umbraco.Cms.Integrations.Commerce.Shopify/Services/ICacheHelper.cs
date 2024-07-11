﻿
namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Services
{
    public interface ICacheHelper
    {
        bool TryGetCachedItem<T>(string key, out T item) where T : class;

        void AddCachedItem(string key, string serializedItem);

        void ClearCachedItems();
    }
}
