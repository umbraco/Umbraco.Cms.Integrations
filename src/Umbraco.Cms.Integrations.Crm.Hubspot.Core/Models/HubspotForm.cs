using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models
{
    public class HubspotForm
    {
        [JsonPropertyName("portalId")]
        public long PortalId { get; set; }

        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("formFieldGroups")]
        public List<FormFieldGroup> FormFieldGroups { get; set; }

        public static HubspotForm FromJson(string json) => JsonSerializer.Deserialize<HubspotForm>(json);
    }
}
