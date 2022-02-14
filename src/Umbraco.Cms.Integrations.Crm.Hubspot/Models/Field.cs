using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
{
    public class Field
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
