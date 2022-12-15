using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class FieldType
    {
        [JsonPropertyName("fieldTypeId")]
        public string Id { get; set; }

        [JsonPropertyName("fieldTypeDisplayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("fieldTypeDescription")]
        public string Description { get; set; }
    }
}
