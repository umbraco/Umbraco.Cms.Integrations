

using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class Entity
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("fields")]
        public IEnumerable<FieldValue> Fields { get; set; }
    }
}
