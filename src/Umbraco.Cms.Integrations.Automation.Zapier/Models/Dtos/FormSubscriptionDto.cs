using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class FormSubscriptionDto
    {
        [JsonProperty("hookUrl")]
        public string HookUrl { get; set; }

        [JsonProperty("formId")]
        public string FormId { get; set; }

        [JsonProperty("subscribeHook")]
        public bool SubscribeHook { get; set; }
    }
}
