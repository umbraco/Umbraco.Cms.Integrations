using System.Linq;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class SubscriptionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("entityId")]
        public string EntityId { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("typeName")]
        public string TypeName
        {
            get
            {
                var entityType = typeof(Constants.EntityType);

                var typeField = entityType.GetFields()
                    .FirstOrDefault(p => p.GetValue(entityType).Equals(Type));
                
                return typeField != null ? typeField.Name : string.Empty;
            }
        }
        
        [JsonProperty("hookUrl")]
        public string HookUrl { get; set; }

        [JsonProperty("subscribeHook")]
        public bool SubscribeHook { get; set; }
    }
}
