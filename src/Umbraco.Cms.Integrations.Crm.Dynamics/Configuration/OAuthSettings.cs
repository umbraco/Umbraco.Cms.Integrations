
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Configuration
{
    public class OAuthSettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string Scopes { get; set; }

        public bool UseDefault { get; set; }
    }
}
