using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration
{
    public class GoogleSearchConsoleSettings
    {
        public GoogleSearchConsoleSettings()
        {
        }

        public GoogleSearchConsoleSettings(NameValueCollection appSettings)
        {
            InspectUrl = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoGoogleSearchConsoleInspectUrlKey];

            UseUmbracoAuthorization = 
                bool.TryParse(appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoGoogleSearchConsoleUseUmbracoAuthorizationKey], out var key)
                    ? key
                    : true;
        }

        public string InspectUrl { get; set; }

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
