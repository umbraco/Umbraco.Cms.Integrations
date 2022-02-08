using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
{
    public class HubspotFormPickerSettings
    {
        public HubspotFormPickerSettings()
        {
            Type = ConfigurationType.None;
        }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("type")]
        public ConfigurationType Type { get; set; }
    }
}
