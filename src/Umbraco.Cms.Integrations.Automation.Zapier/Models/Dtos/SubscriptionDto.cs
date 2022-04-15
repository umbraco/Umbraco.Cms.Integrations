using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class SubscriptionDto
    {
        [JsonProperty("hookUrl")]
        public string HookUrl { get; set; }

        [JsonProperty("enable")]
        public bool Enable { get; set; }
    }
}
