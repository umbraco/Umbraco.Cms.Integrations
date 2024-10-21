using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class RelatedPhrasesDataDto
    {
        [JsonPropertyName("columnNames")]
        public string[] ColumnNames { get; set; }

        [JsonPropertyName("rows")]
        public List<string[]> Rows { get; set; }
    }


}
