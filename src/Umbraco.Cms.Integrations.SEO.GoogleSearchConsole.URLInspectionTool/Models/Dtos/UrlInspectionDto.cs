
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class UrlInspectionDto
    {
        [JsonPropertyName("inspectionUrl")]
        public string InspectionUrl { get; set; }

        [JsonPropertyName("siteUrl")]
        public string SiteUrl { get; set; }

        [JsonPropertyName("languageCode")] 
        public string LanguageCode { get; set; }
    }
}
