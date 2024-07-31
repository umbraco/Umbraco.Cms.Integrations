namespace Umbraco.Cms.Integrations.Crm.Hubspot.Configuration
{
    public class HubspotOAuthSettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
