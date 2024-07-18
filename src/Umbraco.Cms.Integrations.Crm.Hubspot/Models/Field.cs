using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
{
    public class Field
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}
