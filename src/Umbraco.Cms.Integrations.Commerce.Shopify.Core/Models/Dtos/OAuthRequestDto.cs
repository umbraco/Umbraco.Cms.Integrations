using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos
{
    public class OAuthRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
