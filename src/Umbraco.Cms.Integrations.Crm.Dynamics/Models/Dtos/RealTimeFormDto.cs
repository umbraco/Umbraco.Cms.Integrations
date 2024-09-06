
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class RealTimeFormDto
    {
        [JsonPropertyName("msdynmkt_marketingformid")]
        public string Id { get; set; }

        [JsonPropertyName("msdynmkt_name")]
        public string Name { get; set; }

        [JsonPropertyName("msdynmkt_formhtml")]
        public string FormHtml { get; set; }

        [JsonPropertyName("msdynmkt_standalonehtml")]
        public string StandaloneHtml { get; set; }
    }
}
