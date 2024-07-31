export interface HubspotServiceConfig {
    api: string;
    oauth: string;
    oauthConnected: string;
    none: string;
}

export type HubspotServiceStatus = {
    isValid: boolean;
    type: string;
    description: string;
    useOAuth: boolean;
}

export type HubspotOAuthSetup = {
    isConnected?: boolean;
    isAccessTokenExpired?: boolean;
    isAccessTokenValid?: boolean;
}

export const ConfigDescription: HubspotServiceConfig = {
    api: "An API key is configured and will be used to connect to your HubSpot account.",
    oauth: "No API key is configured. To connect to your HubSpot account using OAuth click 'Connect', select your account and agree to the permissions.",
    oauthConnected: "OAuth is configured and an access token is available to connect to your HubSpot account. To revoke, click 'Revoke'",
    none: "No API or OAuth configuration could be found. Please review your settings before continuing."
}

