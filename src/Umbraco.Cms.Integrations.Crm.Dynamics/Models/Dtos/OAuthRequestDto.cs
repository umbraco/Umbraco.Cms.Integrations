
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class OAuthRequestDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
