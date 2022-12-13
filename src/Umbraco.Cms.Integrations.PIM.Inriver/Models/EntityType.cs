
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class EntityType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("fieldTypes")]
        public List<FieldType> FieldTypes { get; set; }
    }
}
