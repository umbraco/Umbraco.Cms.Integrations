using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class OAuthRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
