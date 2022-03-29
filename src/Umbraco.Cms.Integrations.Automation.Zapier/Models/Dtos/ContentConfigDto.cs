using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class ContentConfigDto
    {
        public ContentConfigDto() { }

        public ContentConfigDto(string webHookUrl)
        {
            WebHookUrl = webHookUrl;
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("contentTypeAlias")]
        public string ContentTypeAlias { get; set; }

        [JsonProperty("webHookUrl")]
        public string WebHookUrl { get; set; }
    }
}
