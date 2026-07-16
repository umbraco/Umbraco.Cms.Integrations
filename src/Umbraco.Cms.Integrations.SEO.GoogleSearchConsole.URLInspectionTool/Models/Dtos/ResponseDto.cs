using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class ResponseDto
    {
        [JsonPropertyName("inspectionResult")]
        public InspectionResultDto InspectionResult { get; set; }

        [JsonPropertyName("error")]
        public ErrorDto Error { get; set; }
    }
}
