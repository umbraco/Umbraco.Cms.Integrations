using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Shared.Models.Dtos
{
    public class OAuthRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
