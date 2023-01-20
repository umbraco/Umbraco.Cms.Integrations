using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class FieldType
    {
        [JsonPropertyName("fieldTypeId")]
        [JsonProperty("fieldTypeId")]
        public string Id { get; set; }

        [JsonPropertyName("fieldTypeDisplayName")]
        [JsonProperty("fieldTypeDisplayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("fieldTypeDescription")]
        [JsonProperty("fieldTypeDescription")]
        public string Description { get; set; }
    }
}
