using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos
{
    public class ContentTypeDto
    {
        public int Id { get; set; }

        [JsonPropertyName("contentType")]
        public ContentEntity? ContentType { get; set; }

        public string Icon { get; set; }

        public string Alias { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public bool Selected { get; set; }

        public bool AllowRemove { get; set; }

        [JsonPropertyName("properties")]
        public IEnumerable<ContentTypePropertyDto> Properties { get; set; }
    }
}
