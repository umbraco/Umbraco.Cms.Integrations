namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ErrorDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
