
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos
{
    public class RelatedPhrasesDto
    {
        [JsonProperty("data")]
        public RelatedPhrasesDataDto Data { get; set; }
    }

}
