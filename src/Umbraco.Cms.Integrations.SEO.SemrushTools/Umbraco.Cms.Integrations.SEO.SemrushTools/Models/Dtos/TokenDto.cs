using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos
{
    public class TokenDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public bool IsAccessTokenAvailable => !string.IsNullOrEmpty(AccessToken);
    }
}
