namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductVariantDto
    {
        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("sku")]
        public string Sku { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("inventory_quantity")]
        public int InventoryQuantity { get; set; }
    }
}
