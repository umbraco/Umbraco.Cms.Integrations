using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos
{
    public class HubspotFormDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("portalId")]
        public string PortalId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("fields")]
        public string Fields { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }
    }
}
