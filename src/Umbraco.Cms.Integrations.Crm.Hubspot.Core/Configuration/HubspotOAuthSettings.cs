﻿using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration
{
    public class HubspotOAuthSettings
    {
        public HubspotOAuthSettings() { }

        public HubspotOAuthSettings(NameValueCollection appSettings)
        {
            ClientId = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotClientIdKey];

            ClientSecret = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotClientSecretKey];

            RedirectUri = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotRedirectUriKey];

            Scopes = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotScopesKey];

            TokenEndpoint = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotTokenEndpointKey];
        }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
