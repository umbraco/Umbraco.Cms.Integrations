using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Configuration
{
    public class ShopifySettings
    {
        public string ApiVersion { get; set; } = string.Empty;

        public string Shop { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public bool UseUmbracoAuthorization { get; set; } = true;

        public PropertyCacheLevel PropertyCacheLevel { get; set; } = PropertyCacheLevel.Snapshot;
    }
}
