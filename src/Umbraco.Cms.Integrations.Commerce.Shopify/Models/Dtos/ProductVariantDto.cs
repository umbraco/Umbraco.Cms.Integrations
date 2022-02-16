using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductVariantDto
    {
        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("inventory_quantity")]
        public int InventoryQuantity { get; set; }
    }
}
