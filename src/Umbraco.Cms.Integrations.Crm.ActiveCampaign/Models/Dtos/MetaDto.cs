using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos
{
    public class MetaDto
    {
        [JsonPropertyName("total")]
        public string Total { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages => int.TryParse(Total, out var total) 
            ? (int)Math.Ceiling((double)total / Constants.DefaultPageSize) 
            : 0;
    }
}
