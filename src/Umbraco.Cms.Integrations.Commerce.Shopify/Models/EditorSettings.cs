using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models
{
    public class EditorSettings
    {
        public EditorSettings()
        {
            Type = ConfigurationType.None;
        }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isConnected")]
        public bool IsConnected { get; set; }

        [JsonProperty("type")]
        public ConfigurationType Type { get; set; }
    }
}
