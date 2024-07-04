function shopifyService() {
    return {
        configDescription: {
            API: "An access token is configured and will be used to connect to your Shopify account.",
            OAuth:
                "No access token is configured. To connect to your Shopify account using OAuth click 'Connect', select your account and agree to the permissions.",
            None: "No access token or OAuth configuration could be found. Please review your settings before continuing.",
            OAuthConnected:
                "OAuth is configured and an access token is available to connect to your Shopify account. To revoke, click 'Revoke'"
        }
    }
}

angular.module("umbraco.services")
    .service("umbracoCmsIntegrationsCommerceShopifyService", shopifyService);