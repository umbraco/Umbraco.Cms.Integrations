using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class Record
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("thumbnail")]
        public RecordThumbnail Thumbnail { get; set; }
    }

    public class RecordThumbnail 
    {
        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("extension")]
        public string Extension { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
