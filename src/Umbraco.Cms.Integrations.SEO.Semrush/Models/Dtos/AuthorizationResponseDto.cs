
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class AuthorizationResponseDto
    {
        [JsonProperty("isExpired")]
        public bool IsExpired { get; set; }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isFreeAccount")]
        public bool? IsFreeAccount { get; set; }
    }
}
