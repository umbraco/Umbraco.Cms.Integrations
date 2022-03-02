using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Configuration
{
    public class ShopifySettings
    {
        public ShopifySettings()
        {
            
        }

        public ShopifySettings(NameValueCollection appSettings)
        {
            ApiVersion = appSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyApiVersion];
            Shop = appSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop];
            AccessToken = appSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyAccessToken];
        }

        public string ApiVersion { get; set; }

        public string Shop { get; set; }

        public string AccessToken { get; set; }
    }
}
