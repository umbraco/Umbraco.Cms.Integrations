using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos
{
    public class ProductImageDto
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}
