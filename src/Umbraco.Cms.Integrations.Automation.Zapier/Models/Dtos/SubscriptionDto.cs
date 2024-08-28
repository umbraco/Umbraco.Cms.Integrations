namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class SubscriptionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("entityId")]
        public string EntityId { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("typeName")]
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
        
        [JsonPropertyName("hookUrl")]
        public string HookUrl { get; set; }

        [JsonPropertyName("subscribeHook")]
        public bool SubscribeHook { get; set; }
    }
}
