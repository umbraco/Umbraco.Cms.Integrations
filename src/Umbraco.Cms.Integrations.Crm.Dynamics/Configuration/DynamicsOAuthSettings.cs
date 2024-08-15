namespace Umbraco.Cms.Integrations.Crm.Dynamics.Configuration
{
    public class DynamicsOAuthSettings
    {
        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;

        public string Scopes { get; set; } = string.Empty;

        public string TokenEndpoint { get; set; } = string.Empty;
    }
}
