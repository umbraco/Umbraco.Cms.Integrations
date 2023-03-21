using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class AprimoRequest
    {
        [JsonPropertyName("searchExpression")]
        public AprimoSearchExpression SearchExpression { get; set; }

        [JsonPropertyName("logRequest")]
        public bool LogRequest = true;
    }

    public class AprimoSearchExpression
    {
        [JsonPropertyName("expression")]
        public string Expression { get; set; }
    }
}
