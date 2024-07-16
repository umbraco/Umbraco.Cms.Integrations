using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class RequestDto
    {
        [JsonProperty("ids")]
        public string[] Ids { get; set; }
    }
}
