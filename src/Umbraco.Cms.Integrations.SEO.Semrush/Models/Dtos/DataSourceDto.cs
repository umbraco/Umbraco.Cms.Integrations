using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class DataSourceDto
    {
        public DataSourceDto()
        {
            Items = Enumerable.Empty<DataSourceItemDto>();
        }
        public IEnumerable<DataSourceItemDto> Items { get; set; }
    }

    public class DataSourceItemDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("researchTypes")]
        public string ResearchTypes { get; set; }

        [JsonPropertyName("googleSearchDomain")]
        public string GoogleSearchDomain { get; set; }
    }
}
