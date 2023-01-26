using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class QueryRequest
    {
        [JsonPropertyName("entityTypeId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string EntityTypeId { get; set; }

        [JsonPropertyName("fieldTypes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FieldType[] FieldTypes { get; set; }

        [JsonPropertyName("linkedTypes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] LinkedTypes { get; set; }

        [JsonPropertyName("systemCriteria")]
        public List<Criterion> SystemCriteria { get; set; }

        [JsonPropertyName("culture")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Culture { get; set; }
    }
}
