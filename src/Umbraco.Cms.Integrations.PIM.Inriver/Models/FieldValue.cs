using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class FieldValue
    {
        [JsonPropertyName("fieldTypeId")]
        public string FieldTypeId { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }

        public Dictionary<string, string> ValueDictionary
        {
            get
            {
                try
                {
                    if (Value == null) return null;

                    return JsonSerializer.Deserialize<Dictionary<string, string>>(Value.ToString());
                }
                catch { return null; }
            }
        }

        public string Display
        {
            get
            {
                if(ValueDictionary != null)
                {
                    return string.Join(",", ValueDictionary);
                }

                return Value?.ToString();
            }
        }
    }
}
