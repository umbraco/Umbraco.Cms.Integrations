using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class EntityData
    {
        [JsonPropertyName("entityId")]
        public int EntityId { get; set; }

        [JsonPropertyName("summary")]
        public EntitySummary Summary { get; set; }

        [JsonPropertyName("fieldValues")]
        public IEnumerable<FieldValue> Fields { get; set; }
    }
}
