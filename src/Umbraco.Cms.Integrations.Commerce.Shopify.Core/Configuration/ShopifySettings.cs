using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Configuration
{
    public class ShopifySettings
    {
        public ShopifySettings()
        {
            
        }

        public ShopifySettings(NameValueCollection appSettings)
        {
            ApiVersion = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyApiVersion];
            Shop = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyShop];
            AccessToken = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyAccessToken];
            UseUmbracoAuthorization = bool.TryParse(appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyUseUmbracoAuthorizationKey], out var key)
               ? key
               : true;
        }

        public string ApiVersion { get; set; }

        public string Shop { get; set; }

        public string AccessToken { get; set; }

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
