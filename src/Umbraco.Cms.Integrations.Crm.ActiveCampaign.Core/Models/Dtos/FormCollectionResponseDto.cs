using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Models.Dtos
{
    public class FormCollectionResponseDto : BaseResponseDto
    {
        [JsonPropertyName("forms")]
        public List<FormDto> Form { get; set; }
    }
}
