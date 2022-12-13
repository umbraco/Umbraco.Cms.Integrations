using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Configuration
{
    public class EditorSettings
    {
        [JsonPropertyName("entityType")]
        public string EntityType { get; set; } = string.Empty;

        [JsonPropertyName("allowChange")]
        public bool AllowChange { get; set; }
    }
}
