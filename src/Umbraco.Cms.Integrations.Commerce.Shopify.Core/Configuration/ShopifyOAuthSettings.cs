using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Configuration
{
    public class ShopifyOAuthSettings
    {
        public ShopifyOAuthSettings() { }

        public ShopifyOAuthSettings(NameValueCollection appSettings)
        {
            ClientId = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyClientIdKey];

            ClientSecret = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyClientSecretKey];

            RedirectUri = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyRedirectUriKey];

            Scopes = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyScopesKey];

            TokenEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyTokenEndpointKey];
        }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
