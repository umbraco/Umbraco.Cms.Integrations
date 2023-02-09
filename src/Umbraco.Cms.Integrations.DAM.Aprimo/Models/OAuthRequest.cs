using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class OAuthRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
