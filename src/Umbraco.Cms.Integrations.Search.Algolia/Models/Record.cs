using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class Record
    {
        public Record()
        {
            Data = new Dictionary<string, string>();
        }

        public string ObjectID { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CreateDate { get; set; }

        public string UpdateDate { get; set; }

        public string Url { get; set; }

        public Dictionary<string, string> Data { get; set; }
    }
}
