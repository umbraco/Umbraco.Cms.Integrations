using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos
{
    public abstract class BaseResponseDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
