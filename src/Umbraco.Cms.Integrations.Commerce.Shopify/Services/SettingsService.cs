
namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public class SettingsService
    {
        public static string OAuthProxyBaseUrl = "https://localhost:44364/";
        
        public static string OAuthProxyEndpoint = "{0}oauth/v1/token";

        public static string ShopifyOAuthProxyUrl = $"{OAuthProxyBaseUrl}oauth/shopify";

        public static string OAuthClientId = "23c1bc3c70de807d84b79a29b12b49f5";

        public static string AccessTokenDbKey = "Umbraco.Cms.Integrations.Shopify.AccessTokenDbKey";

        public static string ServiceName = "Shopify";
        
        public static string ServiceAddressReplace = "service_address_shop-replace";
        
        public static string AuthorizationUrl =
            "https://{0}.myshopify.com/admin/oauth/authorize?client_id={1}&redirect_uri={2}&scope=read_products&grant_options[]=value";

        public static string ProductsApiEndpoint = "https://{0}.myshopify.com/admin/api/{1}/products.json";
    }
}
