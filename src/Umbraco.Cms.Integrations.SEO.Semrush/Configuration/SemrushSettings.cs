using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Configuration
{
    public class SemrushSettings
    {
        public SemrushSettings() { }

        public SemrushSettings(NameValueCollection appSettings)
        {
            BaseUrl = appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushBaseUrlKey]; 

            UseUmbracoAuthorization = bool.TryParse(appSettings[Constants.Configuration.UmbracoCmsIntegrationsSeoSemrushUseUmbracoAuthorizationKey], 
                out var key)
               ? key
               : true;
        }

        public string BaseUrl { get; set; }

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
