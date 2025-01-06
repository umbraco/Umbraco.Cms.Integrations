using System.Collections.Specialized;
#if NETCOREAPP
using Umbraco.Cms.Core.PropertyEditors;
#else
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Configuration
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

            // Enum.TryParse introduced in .NET site is .NETFramework and .NET Core, same logic will work for both
            switch (appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyPropertyCacheLevel])
            {
                case "Unknown":
                    PropertyCacheLevel = PropertyCacheLevel.Unknown;
                    break;
                case "Element":
                    PropertyCacheLevel = PropertyCacheLevel.Element;
                    break;
                case "Elements":
                    PropertyCacheLevel = PropertyCacheLevel.Elements;
                    break;
                case "None":
                    PropertyCacheLevel = PropertyCacheLevel.None;
                    break;
                default:
                    PropertyCacheLevel = PropertyCacheLevel.Snapshot;
                    break;
            }
        }

        public string ApiVersion { get; set; }

        public string Shop { get; set; }

        public string AccessToken { get; set; }

        public bool UseUmbracoAuthorization { get; set; } = true;

        public PropertyCacheLevel PropertyCacheLevel { get; set; } = PropertyCacheLevel.Snapshot;
    }
}
