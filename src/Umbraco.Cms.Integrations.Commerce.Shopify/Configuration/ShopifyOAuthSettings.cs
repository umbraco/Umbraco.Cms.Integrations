namespace Umbraco.Cms.Integrations.Commerce.Shopify.Configuration
{
    public class ShopifyOAuthSettings
    {
        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;

        public string Scopes { get; set; } = string.Empty;

        public string TokenEndpoint { get; set; } = string.Empty;
    }
}
