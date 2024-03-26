using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class RealTimeFormDto
    {
        [JsonProperty("msdynmkt_marketingformid")]
        public string Id { get; set; }

        [JsonProperty("msdynmkt_name")]
        public string Name { get; set; }

        [JsonProperty("msdynmkt_formhtml")]
        public string FormHtml { get; set; }
    }
}
