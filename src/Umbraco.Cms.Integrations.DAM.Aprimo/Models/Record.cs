using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class Record
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("thumbnail")]
        public RecordThumbnail Thumbnail { get; set; }

        [JsonPropertyName("masterFileLatestVersion")]
        public RecordMasterFile MasterFileLatestVersion { get; set; }

        [JsonPropertyName("fields")]
        public RecordFields Fields { get; set;}
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

    public class RecordMasterFile
    {
        [JsonPropertyName("renditions")]
        public RecordRenditions Renditions { get; set; }
    }

    public class RecordRenditions
    {
        [JsonPropertyName("items")]
        public IEnumerable<RecordRenditionItem> Items { get; set; }
    }

    public class RecordRenditionItem
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("resizeWidth")]
        public int ResizeWidth { get; set; }

        [JsonPropertyName("resizeHeight")]
        public int ResizeHeight { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("presetName")]
        public string PresetName { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("extension")]
        public string Extension { get; set; }

        [JsonPropertyName("publicLinks")]
        public RenditionPublicLinks PublicLinks { get; set; } = new RenditionPublicLinks();
    }

    public class RecordFields
    {
        [JsonPropertyName("items")]
        public IEnumerable<RecordFieldItem> Items { get; set;}
    }

    public class RecordFieldItem
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("localizedValues")]
        public IEnumerable<RecordFieldItemValue> Values { get; set; }
    }

    public class RecordFieldItemValue
    {
        [JsonPropertyName("languageId")]
        public string LanguageId { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("values")]
        public string[] Values { get; set; }
    }

    public class RenditionPublicLinks
    {
        public RenditionPublicLinks()
        {
            Items = Enumerable.Empty<PublicLinkItem>();
        }

        [JsonPropertyName("items")]
        public IEnumerable<PublicLinkItem> Items { get; set; }
    }

    public class PublicLinkItem
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}
