using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Shared.Models
{
    public class EditorSettings
    {
        public EditorSettings()
        {
            Type = ConfigurationType.None;
        }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("type")]
        public ConfigurationType Type { get; set; }
    }
}
