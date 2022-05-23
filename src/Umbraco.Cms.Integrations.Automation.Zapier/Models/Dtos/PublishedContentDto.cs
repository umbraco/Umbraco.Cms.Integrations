using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class PublishedContentDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("publishDate")]
        public string PublishDate { get; set; }
    }
}
