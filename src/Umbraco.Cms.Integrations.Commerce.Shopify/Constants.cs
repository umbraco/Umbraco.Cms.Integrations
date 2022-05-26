
namespace Umbraco.Cms.Integrations.Commerce.Shopify
{
    public static class Constants
    {
        public const string UmbracoCmsIntegrationsCommerceShopifyApiVersion =
            "Umbraco.Cms.Integrations.Commerce.Shopify.ApiVersion";

        public const string UmbracoCmsIntegrationsCommerceShopifyShop =
            "Umbraco.Cms.Integrations.Commerce.Shopify.Shop";

        public const string UmbracoCmsIntegrationsCommerceShopifyAccessToken =
            "Umbraco.Cms.Integrations.Commerce.Shopify.AccessToken";

        public const string AppPluginFolderPath = "~/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify";

        public static class RenderingComponent
        {
            public const string DefaultV8ViewPath = AppPluginFolderPath + "/Render/Products.cshtml";

            public const string DefaultV9ViewPath = AppPluginFolderPath + "/Render/ProductsV9.cshtml";
        }

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:Commerce:Shopify:Settings";
        }

        public static class PropertyEditors
        {
            public const string ProductPickerAlias = "Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker";
        }
    }
}
