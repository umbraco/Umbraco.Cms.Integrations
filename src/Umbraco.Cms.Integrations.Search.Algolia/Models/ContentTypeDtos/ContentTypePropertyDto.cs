using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos
{
    public class ContentTypePropertyDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("selected")]
        public bool Selected { get; set; }
    }
}
