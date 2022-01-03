using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos
{
    public class RelatedPhrasesDto: ErrorDto
    {
        [JsonProperty("data")]
        public RelatedPhrasesDataDto Data { get; set; }

        public int TotalPages { get; set; }
    }

}
