using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class FieldValue
    {
        [JsonPropertyName("fieldTypeId")]
        public string FieldTypeId { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
