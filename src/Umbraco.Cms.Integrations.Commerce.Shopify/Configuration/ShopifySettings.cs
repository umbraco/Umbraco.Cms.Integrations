namespace Umbraco.Cms.Integrations.Commerce.Shopify.Configuration
{
    public class ShopifySettings
    {
        public string ApiVersion { get; set; } = string.Empty;

        public string Shop { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
