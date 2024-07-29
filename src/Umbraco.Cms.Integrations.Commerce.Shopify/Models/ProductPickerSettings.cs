namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models
{
    public class ProductPickerSettings
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
