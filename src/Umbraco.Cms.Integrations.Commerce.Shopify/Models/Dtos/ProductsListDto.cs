using System.Collections.Generic;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductsListDto
    {
        public IEnumerable<ProductDto> Products { get; set; }
    }
}
