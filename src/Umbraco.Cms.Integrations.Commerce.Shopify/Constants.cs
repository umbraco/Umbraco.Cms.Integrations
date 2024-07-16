
namespace Umbraco.Cms.Integrations.Commerce.Shopify
{
    public static class Constants
    {
        public const string AppPluginFolderPath = "~/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify";

        public const string AccessTokenDbKey = "Umbraco.Cms.Integrations.Shopify.AccessTokenDbKey";

        public const string ProductsApiEndpoint = "https://{0}.myshopify.com/admin/api/{1}/products.json";

        public const string ProductsCountApiEndpoint = "https://{0}.myshopify.com/admin/api/{1}/products/count.json";

        public const string RefreshTokenDbKey = "Umbraco.Cms.Integrations.Shopify.RefreshTokenDbKey";

        public const int DEFAULT_PAGE_SIZE = 10;

        public static class RenderingComponent
        {
            public const string DefaultV8ViewPath = AppPluginFolderPath + "/Render/Products.cshtml";

            public const string DefaultV9ViewPath = AppPluginFolderPath + "/Render/ProductsV9.cshtml";
        }

        public static class ManagementApi
        {
            public const string RootPath = "shopify/management/api";

            public const string ApiTitle = "Shopify Management API";

            public const string ApiName = "shopify-management";

            public const string GroupName = "Shopify";
        }

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:Commerce:Shopify:Settings";

            public const string OAuthSettings = "Umbraco:Cms:Integrations:Commerce:Shopify:OAuthSettings";

            public const string UmbracoCmsIntegrationsCommerceShopifyApiVersion =
            "Umbraco.Cms.Integrations.Commerce.Shopify.ApiVersion";

            public const string UmbracoCmsIntegrationsCommerceShopifyShop =
                "Umbraco.Cms.Integrations.Commerce.Shopify.Shop";

            public const string UmbracoCmsIntegrationsCommerceShopifyAccessToken =
                "Umbraco.Cms.Integrations.Commerce.Shopify.AccessToken";

            public const string UmbracoCmsIntegrationsCommerceShopifyUseUmbracoAuthorizationKey =
                "Umbraco.Cms.Integrations.Commerce.Shopify.UseUmbracoAuthorization";

            public const string UmbracoCmsIntegrationsCommerceShopifyClientIdKey = "Umbraco.Cms.Integrations.Commerce.Shopify.ClientId";

            public const string UmbracoCmsIntegrationsCommerceShopifyClientSecretKey = "Umbraco.Cms.Integrations.Commerce.Shopify.ClientSecret";

            public const string UmbracoCmsIntegrationsCommerceShopifyRedirectUriKey = "Umbraco.Cms.Integrations.Commerce.Shopify.RedirectUri";

            public const string UmbracoCmsIntegrationsCommerceShopifyScopesKey = "Umbraco.Cms.Integrations.Commerce.Shopify.Scopes";

            public const string UmbracoCmsIntegrationsCommerceShopifyTokenEndpointKey = "Umbraco.Cms.Integrations.Commerce.Shopify.TokenEndpoint";
        }

        public static class PropertyEditors
        {
            public const string ProductPickerAlias = "Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker";
        }
    }
}
