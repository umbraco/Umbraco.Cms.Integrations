using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos
{
    public class ProductsListDto
    {
        [JsonProperty("products")]
        public IEnumerable<ProductDto> Products { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
    }
}
