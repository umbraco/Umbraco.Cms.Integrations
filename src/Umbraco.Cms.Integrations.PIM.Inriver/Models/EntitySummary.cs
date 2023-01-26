

using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class EntitySummary
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("resourceUrl")]
        public string ResourceUrl { get; set; }
    }
}
