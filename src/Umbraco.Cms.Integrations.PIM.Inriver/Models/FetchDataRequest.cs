
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class FetchDataRequest
    {
        [JsonPropertyName("entityIds")]
        public int[] EntityIds { get; set; }

        [JsonPropertyName("objects")]
        public string Objects { get; } = "EntitySummary,FieldValues";

        [JsonPropertyName("fieldTypeIds")]
        public string FieldTypeIds { get; set; }
    }
}
