using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos
{
    public class ErrorDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
