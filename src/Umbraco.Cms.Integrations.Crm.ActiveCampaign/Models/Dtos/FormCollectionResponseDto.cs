using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Models.Dtos
{
    public class FormCollectionResponseDto : BaseResponseDto
    {
        [JsonPropertyName("forms")]
        public List<FormDto> Form { get; set; }

        [JsonPropertyName("meta")]
        public MetaDto Meta { get; set; }
    }
}
