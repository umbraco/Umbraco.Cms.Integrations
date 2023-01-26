using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class Criterion
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "EntityTypeId";

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("operator")]
        public string Operator { get; set; } = "Equal";

        public Criterion(string value)
        {
            Value = value;
        }
    }
}
