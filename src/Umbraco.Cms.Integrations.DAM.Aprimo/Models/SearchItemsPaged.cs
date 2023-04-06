using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class SearchItemsPaged<T> where T: class
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("items")]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
