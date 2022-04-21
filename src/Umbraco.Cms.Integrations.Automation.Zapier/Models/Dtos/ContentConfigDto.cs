using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class ContentConfigDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("contentTypeAlias")]
        public string ContentTypeAlias { get; set; }

        [JsonProperty("contentTypeName")]
        public string ContentTypeName { get; set; }

        [JsonProperty("hookUrl")]
        public string HookUrl { get; set; }
    }
}
