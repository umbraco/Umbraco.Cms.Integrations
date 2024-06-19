using Newtonsoft.Json;
using System.Collections.Generic;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body_html")]
        public string Body { get; set; }

        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("variants")]
        public IEnumerable<ProductVariantDto> Variants { get; set; }

        [JsonProperty("image")]
        public ProductImageDto Image { get; set; }

        [JsonProperty("product_type")]
        public string ProductType { get; set; }

        [JsonProperty("published_scope")]
        public string PublishedScope { get; set; }
    }


}
