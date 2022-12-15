using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Configuration
{
    public class EditorSettings
    {
        [JsonProperty("entityType")]
        public string EntityType { get; set; } = string.Empty;

        [JsonProperty("displayFieldTypeIds")]
        public string[] DisplayFieldTypeIds { get; set; }
    }
}
