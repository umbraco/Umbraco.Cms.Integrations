
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos
{
    public class AuthorizationRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
