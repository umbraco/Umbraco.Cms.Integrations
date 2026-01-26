namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration
{
    public class GoogleSearchConsoleOAuthSettings
    {
        public GoogleSearchConsoleOAuthSettings() { }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string TokenEndpoint { get; set; }
    }
}
