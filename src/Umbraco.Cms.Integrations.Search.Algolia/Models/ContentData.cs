using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class ContentData
    {
        [JsonPropertyName("contentType")]
        public ContentEntity ContentType { get; set; }

        [JsonPropertyName("properties")]
        public IEnumerable<ContentEntity> Properties { get; set; }

        [JsonPropertyName("propertiesDescription")]
        public IEnumerable<string> PropertiesDescription => Properties.Select(p => p.Name);
    }
}
