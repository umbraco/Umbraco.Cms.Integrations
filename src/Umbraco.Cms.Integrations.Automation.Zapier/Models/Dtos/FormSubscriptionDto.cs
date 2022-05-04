using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class FormSubscriptionDto
    {
        [JsonProperty("hookUrl")]
        public string HookUrl { get; set; }

        [JsonProperty("formName")]
        public string FormName { get; set; }

        [JsonProperty("subscribeHook")]
        public bool SubscribeHook { get; set; }
    }
}
