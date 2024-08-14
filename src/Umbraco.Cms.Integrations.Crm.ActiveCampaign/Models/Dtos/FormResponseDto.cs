using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos
{
    public class FormResponseDto : BaseResponseDto
    {
        [JsonPropertyName("form")]
        public FormDto Form { get; set; }
    }
}
