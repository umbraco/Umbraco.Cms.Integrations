namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("body_html")]
        public string Body { get; set; }

        [JsonPropertyName("vendor")]
        public string Vendor { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("variants")]
        public IEnumerable<ProductVariantDto> Variants { get; set; }

        [JsonPropertyName("image")]
        public ProductImageDto Image { get; set; }
    }


}
