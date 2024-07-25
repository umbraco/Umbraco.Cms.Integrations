using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class RequestDto
    {
        [JsonProperty("ids")]
        public long[] Ids { get; set; }
    }
}
