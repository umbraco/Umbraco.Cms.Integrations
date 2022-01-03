using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos
{
    public class ErrorDto
    {
        public bool IsSuccessful => string.IsNullOrWhiteSpace(Error);

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
