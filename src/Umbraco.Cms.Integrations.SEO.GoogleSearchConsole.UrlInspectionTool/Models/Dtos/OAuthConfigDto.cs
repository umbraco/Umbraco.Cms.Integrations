using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class OAuthConfigDto
    {
        [JsonProperty("isConnected")]
        public bool IsConnected { get; set; }

        [JsonProperty("authorizationUrl")]
        public string AuthorizationUrl { get; set; }
    }
}
