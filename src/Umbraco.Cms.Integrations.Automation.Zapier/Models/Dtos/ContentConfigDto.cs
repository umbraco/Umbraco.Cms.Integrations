using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class ContentConfigDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("contentTypeName")]
        public string ContentTypeName { get; set; }

        [JsonProperty("webHookUrl")]
        public string WebHookUrl { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("showDeletePrompt")]
        public bool ShowDeletePrompt { get; set; }
    }
}
