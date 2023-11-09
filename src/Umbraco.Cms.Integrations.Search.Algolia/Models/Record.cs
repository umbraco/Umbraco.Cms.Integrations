using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class Record
    {
        public Record()
        {
            Data = new Dictionary<string, object>();
        }

        public string ObjectID { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CreateDate { get; set; }

        public string CreatorName { get; set; }

        public string UpdateDate { get; set; }

        public string WriterName { get; set; }

        public int TemplateId { get; set; }

        public int Level { get; set; }

        public string Path { get; set; }

        public string ContentTypeAlias { get; set; }

        public string Url { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
