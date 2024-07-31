﻿
namespace Umbraco.Cms.Integrations.Commerce.Shopify
{
    public static class Constants
    {
        public const string ViewFolderPath = "~/Views/Shopify";
        public const string AppPluginFolderPath = "~/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify";

        public const string AccessTokenDbKey = "Umbraco.Cms.Integrations.Shopify.AccessTokenDbKey";

        public const string ProductsApiEndpoint = "https://{0}.myshopify.com/admin/api/{1}/products.json";

        public const string ProductsCountApiEndpoint = "https://{0}.myshopify.com/admin/api/{1}/products/count.json";

        public const string RefreshTokenDbKey = "Umbraco.Cms.Integrations.Shopify.RefreshTokenDbKey";

        public const int DEFAULT_PAGE_SIZE = 10;

        public static class RenderingComponent
        {
            public const string DefaultViewPath = ViewFolderPath + "/Render/Products.cshtml";
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

            public const string UmbracoCmsIntegrationsCommerceShopifyAccessToken =
                "Umbraco.Cms.Integrations.Commerce.Shopify.AccessToken";
        }

        public static class PropertyEditors
        {
            public const string ProductPickerAlias = "Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker";
        }
    }
}
