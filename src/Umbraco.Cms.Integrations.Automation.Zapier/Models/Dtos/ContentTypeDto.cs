using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class ContentTypeDto
    {

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
