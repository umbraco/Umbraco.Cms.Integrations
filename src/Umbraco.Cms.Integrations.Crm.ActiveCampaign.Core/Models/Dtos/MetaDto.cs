using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Models.Dtos
{
    public class MetaDto
    {
        [JsonPropertyName("total")]
        public string Total { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages => int.TryParse(Total, out var total) ? total / Constants.DEFAULT_PAGE_SIZE : 0;
    }
}
