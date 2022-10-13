using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos
{
    public class ResponseDto
    {
        [JsonPropertyName("form")]
        public FormDto Form { get; set; }

        [JsonPropertyName("forms")]
        public List<FormDto> Forms { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
