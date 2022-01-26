using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
{
    public class HubspotFormPickerSettings
    {
        [JsonProperty("useApi")]
        public bool UseApi { get; set; }

        [JsonProperty("useOAuth")]
        public bool UseOAuth { get; set; }
    }
}
