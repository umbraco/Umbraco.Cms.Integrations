using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class ResponseDto
    {
        [JsonProperty("inspectionResult")]
        public InspectionResultDto InspectionResult { get; set; }

        [JsonProperty("error")]
        public ErrorDto Error { get; set; }
    }
}
