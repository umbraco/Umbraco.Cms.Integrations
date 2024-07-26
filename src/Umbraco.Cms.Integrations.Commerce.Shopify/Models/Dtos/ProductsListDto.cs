namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductsListDto
    {
        [JsonPropertyName("products")]
        public IEnumerable<ProductDto> Products { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
}
