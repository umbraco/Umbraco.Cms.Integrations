using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class AuthorizationRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
