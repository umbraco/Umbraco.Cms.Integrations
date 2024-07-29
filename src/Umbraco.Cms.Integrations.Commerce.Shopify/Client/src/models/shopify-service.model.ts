export interface ShopifyServiceConfig {
    api: string;
    oauth: string;
    oauthConnected: string;
    none: string;
}

export type ShopifyServiceStatus = {
    isValid: boolean;
    type: string;
    description: string;
    useOAuth: boolean;
}

export type ShopifyOAuthSetup = {
    isConnected?: boolean;
    isAccessTokenExpired?: boolean;
    isAccessTokenValid?: boolean;
}

export const ConfigDescription: ShopifyServiceConfig = {
    api: "An access token is configured and will be used to connect to your Shopify account.",
    oauth: "No access token is configured. To connect to your Shopify account using OAuth click 'Connect', select your account and agree to the permissions.",
    none: "No access token or OAuth configuration could be found. Please review your settings before continuing.",
    oauthConnected: "OAuth is configured and an access token is available to connect to your Shopify account. To revoke, click 'Revoke'"
}