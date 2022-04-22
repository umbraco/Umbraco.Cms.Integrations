using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class UrlInspectionDto
    {
        [JsonProperty("inspectionUrl")]
        public string InspectionUrl { get; set; }

        [JsonProperty("siteUrl")]
        public string SiteUrl { get; set; }

        [JsonProperty("languageCode")] 
        public string LanguageCode { get; set; }
    }
}
