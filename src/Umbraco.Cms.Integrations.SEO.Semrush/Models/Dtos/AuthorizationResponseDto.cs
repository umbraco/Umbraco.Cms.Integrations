
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class AuthorizationResponseDto
    {
        [JsonProperty("isAuthorized")]
        public bool IsAuthorized { get; set; }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isFreeAccount")]
        public bool? IsFreeAccount { get; set; }
    }
}
