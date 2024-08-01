
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class OutboundFormDto
    {
        [JsonPropertyName("msdyncrm_marketingformid")]
        public string Id { get; set; }

        [JsonPropertyName("msdyncrm_name")]
        public string Name { get; set; }

        [JsonPropertyName("msdyncrm_formdefinition")]
        public string Definition { get; set; }

        [JsonPropertyName("msdyncrm_javascriptcode")]
        public string EmbedCode { get; set; }
    }
}
