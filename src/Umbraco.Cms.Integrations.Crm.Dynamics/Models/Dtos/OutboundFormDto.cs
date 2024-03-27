using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class OutboundFormDto
    {
        [JsonProperty("msdyncrm_marketingformid")]
        public string Id { get; set; }

        [JsonProperty("msdyncrm_name")]
        public string Name { get; set; }

        [JsonProperty("msdyncrm_formdefinition")]
        public string Definition { get; set; }

        [JsonProperty("msdyncrm_javascriptcode")]
        public string EmbedCode { get; set; }
    }
}
