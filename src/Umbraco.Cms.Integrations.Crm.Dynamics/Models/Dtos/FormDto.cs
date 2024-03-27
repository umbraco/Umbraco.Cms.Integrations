using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class FormDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rawHtml")]
        public string RawHtml { get; set; }

        [JsonProperty("standaloneHtml")]
        public string StandaloneHtml { get; set; }

        [JsonProperty("module")]
        public DynamicsModule Module { get; set; }
    }
}
