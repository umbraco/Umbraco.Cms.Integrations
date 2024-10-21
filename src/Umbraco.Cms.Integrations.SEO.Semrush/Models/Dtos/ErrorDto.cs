using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class ErrorDto
    {
        [JsonPropertyName("isSuccessful")]
        public bool IsSuccessful => string.IsNullOrWhiteSpace(Error);

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
