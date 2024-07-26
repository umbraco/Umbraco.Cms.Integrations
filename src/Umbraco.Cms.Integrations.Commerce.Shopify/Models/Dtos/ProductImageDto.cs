namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductImageDto
    {
        [JsonPropertyName("src")]
        public string Src { get; set; }

        [JsonPropertyName("alt")]
        public string Alt { get; set; }
    }
}
