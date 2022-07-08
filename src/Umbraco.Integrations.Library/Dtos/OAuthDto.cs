
namespace Umbraco.Integrations.Library.Dtos
{
    public class OAuthDto
    {
        public bool IsTestingMode { get; set; }

        public string ClientId { get; set; }

        public string ServiceAuthorizationUrl { get; set; }

        public string Scopes { get; set; }

        public string ServiceName { get; set; }

        public const string OAuthProxyTokenEndpoint = "{0}oauth/v1/token";

        public string OAuthProxyBaseUrl => IsTestingMode ? "https://localhost:44364" : "https://hubspot-forms-auth.umbraco.com/";

        public string RedirectUri { get; set; }
    }
}
