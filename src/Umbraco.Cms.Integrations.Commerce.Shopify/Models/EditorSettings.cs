namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models
{
    public class EditorSettings
    {
        public EditorSettings()
        {
            Type = ConfigurationType.None;
        }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("type")]
        public ConfigurationType Type { get; set; }
    }
}
