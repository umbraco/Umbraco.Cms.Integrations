using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class IndexConfiguration
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<ContentTypeDto> ContentData { get; set; }
    }
}
