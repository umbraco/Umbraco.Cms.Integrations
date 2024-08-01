
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class FormDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("rawHtml")]
        public string RawHtml { get; set; }

        [JsonPropertyName("standaloneHtml")]
        public string StandaloneHtml { get; set; }

        [JsonPropertyName("module")]
        public DynamicsModule Module { get; set; }
    }
}
