using System.Collections.Generic;
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models
{
    public class HubspotForm
    {
        [JsonProperty("portalId")]
        public long PortalId { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("formFieldGroups")]
        public List<FormFieldGroup> FormFieldGroups { get; set; }

        public static HubspotForm FromJson(string json) => JsonConvert.DeserializeObject<HubspotForm>(json, Constants.SerializationSettings);
    }
}
