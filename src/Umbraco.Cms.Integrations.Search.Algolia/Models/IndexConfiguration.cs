namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class IndexConfiguration
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ContentData> ContentData { get; set; }
    }
}
