using Newtonsoft.Json;

namespace Umbraco.Integrations.Library.Dtos
{
    public class OAuthRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
