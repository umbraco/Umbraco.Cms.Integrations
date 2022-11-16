
namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class Response
    {
        public int ItemsCount { get; set; }

        public int PagesCount { get; set; }

        public int ItemsPerPage { get; set; }

        public List<Dictionary<string, string>> Hits { get; set; }
    }
}
