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

        public Dictionary<string, string> Data { get; set; }
    }
}
