using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Configuration
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

            //AuthorizationEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyAuthorizationEndpointKey];

            TokenEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyTokenEndpointKey];
        }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        //public string AuthorizationEndpoint { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
