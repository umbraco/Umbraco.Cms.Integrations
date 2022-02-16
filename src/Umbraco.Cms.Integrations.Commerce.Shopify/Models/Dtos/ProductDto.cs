using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body_html")]
        public string Body { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("variants")]
        public IEnumerable<ProductVariantDto> Variants { get; set; }

        [JsonProperty("image")]
        public ProductImageDto Image { get; set; }
    }


}
