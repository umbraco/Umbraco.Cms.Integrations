namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class OAuthRequestDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
