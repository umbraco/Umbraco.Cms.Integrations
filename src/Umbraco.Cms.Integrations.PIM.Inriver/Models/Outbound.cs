using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class Outbound
    {
        [JsonPropertyName("linkTypeIds")]
        public string LinkTypeIds { get; set; }

        [JsonPropertyName("objects")]
        public string Objects { get; } = "EntitySummary,FieldValues";

        [JsonPropertyName("linkEntityObjects")]
        public string LinkEntityObjects { get; } = "FieldValues";
    }
}
