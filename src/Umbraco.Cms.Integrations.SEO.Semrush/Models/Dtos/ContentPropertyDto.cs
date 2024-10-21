using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class ContentPropertyDto
    {
        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; }

        [JsonPropertyName("propertyValue")]
        public string PropertyValue { get; set; }

        [JsonPropertyName("propertyGroup")]
        public string PropertyGroup { get; set; }
    }
}
