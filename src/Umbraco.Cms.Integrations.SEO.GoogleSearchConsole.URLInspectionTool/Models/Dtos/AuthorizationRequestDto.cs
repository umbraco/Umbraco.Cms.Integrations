using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class AuthorizationRequestDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
