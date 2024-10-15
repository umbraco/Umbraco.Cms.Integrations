using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Configuration
{
    public class SemrushSettings
    {
        public SemrushSettings() { }

        public string BaseUrl { get; set; }

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
