function hubspotService() {
    return {
        configDescription: {
            API: "An API key is configured and will be used to connect to your HubSpot account.",
            OAuth:
                "No API key is configured. To connect to your HubSpot account using OAuth click 'Connect', select your account and agree to the permissions.",
            None: "No API or OAuth configuration could be found. Please review your settings before continuing.",
            OAuthConnected:
                "OAuth is configured and an access token is available to connect to your HubSpot account. To revoke, click 'Revoke'"
        }
    }
}

angular.module("umbraco.services")
    .service("umbracoCmsIntegrationsCrmHubspotService", hubspotService);