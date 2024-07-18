using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos
{
    public class OAuthRequestDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
