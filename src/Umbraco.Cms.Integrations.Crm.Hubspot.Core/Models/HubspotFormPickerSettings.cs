using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models
{
    public class HubspotFormPickerSettings
    {
        public HubspotFormPickerSettings()
        {
            Type = ConfigurationType.None;
        }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("type")]
        public ConfigurationType Type { get; set; }
    }
}
