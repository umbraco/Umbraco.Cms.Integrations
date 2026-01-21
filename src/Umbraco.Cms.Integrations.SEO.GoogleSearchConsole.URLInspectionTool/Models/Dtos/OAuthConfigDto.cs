using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class OAuthConfigDto
    {
        [JsonPropertyName("isConnected")]
        public bool IsConnected { get; set; }

        [JsonPropertyName("authorizationUrl")]
        public string AuthorizationUrl { get; set; }
    }
}
