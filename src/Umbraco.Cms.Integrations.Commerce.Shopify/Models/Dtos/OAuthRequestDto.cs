using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class OAuthRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
