using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductsListDto
    {
        [JsonProperty("products")]
        public IEnumerable<ProductDto> Products { get; set; }
    }
}
