using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class QueryRequest
    {
        [JsonPropertyName("entityTypeId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string EntityTypeId { get; set; }

        [JsonPropertyName("fieldTypeIds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FieldTypeIds { get; set; }

        [JsonPropertyName("systemCriteria")]
        public List<Criterion> SystemCriteria { get; set; }
    }
}
