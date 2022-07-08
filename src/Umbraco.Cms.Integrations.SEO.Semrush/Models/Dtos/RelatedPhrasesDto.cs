using Newtonsoft.Json;
using Umbraco.Integrations.Library.Dtos;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class RelatedPhrasesDto: ErrorDto
    {
        [JsonProperty("data")]
        public RelatedPhrasesDataDto Data { get; set; }

        public int TotalPages { get; set; }
    }

}
