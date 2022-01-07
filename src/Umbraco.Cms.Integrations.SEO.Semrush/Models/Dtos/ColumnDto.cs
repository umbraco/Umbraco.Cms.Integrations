using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class ColumnDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
