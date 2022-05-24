using System;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class SubscriptionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("entityId")]
        public string EntityId { get; set; }

        [JsonProperty("hookUrl")]
        public string HookUrl { get; set; }

        [JsonProperty("subscribeHook")]
        public bool SubscribeHook { get; set; }

        [JsonProperty("isFormSubscription")]
        public bool IsFormSubscription => Guid.TryParse(EntityId, out _);
    }
}
