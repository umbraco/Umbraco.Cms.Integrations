using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos
{
    public class HubspotFormDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("portalId")]
        public string PortalId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }

        [JsonProperty("euRegion")]
        public bool EuRegion { get; set; }
    }
}
