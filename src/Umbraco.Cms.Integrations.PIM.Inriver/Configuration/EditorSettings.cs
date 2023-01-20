using Newtonsoft.Json;

using Umbraco.Cms.Integrations.PIM.Inriver.Models;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Configuration
{
    public class EditorSettings
    {
        [JsonProperty("entityType")]
        public string EntityType { get; set; } = string.Empty;

        [JsonProperty("fieldTypes")]
        public FieldType[] FieldTypes { get; set; }
    }
}
