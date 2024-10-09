using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class AuthorizationResponseDto
    {
        [JsonPropertyName("isAuthorized")]
        public bool IsAuthorized { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("isFreeAccount")]
        public bool? IsFreeAccount { get; set; }
    }
}
