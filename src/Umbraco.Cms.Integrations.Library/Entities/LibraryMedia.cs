using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Library.Entities
{
    public class LibraryMedia
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("mediaKey")]
        public string MediaKey { get; set; } = string.Empty;
    }
}
