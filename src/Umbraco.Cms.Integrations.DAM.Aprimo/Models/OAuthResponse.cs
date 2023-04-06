using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class OAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
