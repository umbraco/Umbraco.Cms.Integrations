
using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Configuration
{
    public class SemrushOAuthSettings
    {
        public SemrushOAuthSettings()
        { }

        public string Ref { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
