using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class ContentTypeDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
