using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ProductImageDto
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}
