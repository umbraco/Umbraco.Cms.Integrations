using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class RelatedPhrasesDto: ErrorDto
    {
        [JsonPropertyName("data")]
        public RelatedPhrasesDataDto Data { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        public int TotalPages { get; set; }
    }

}
