using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class Record
    {
        public Record()
        {
            Data = new Dictionary<string, object>();
            GeolocationData = new List<GeolocationEntity>();
        }

        public Record(Record record)
        {
            ContentTypeAlias = record.ContentTypeAlias;
            ObjectID = record.ObjectID;
            Id = record.Id;
            Name = record.Name;
            CreateDate = record.CreateDate;
            CreatorName = record.CreatorName;
            UpdateDate = record.UpdateDate;
            WriterName = record.WriterName;
            TemplateId = record.TemplateId;
            Level = record.Level;
            Path = record.Path;
            Url = record.Url;
            Data = record.Data;
            GeolocationData = record.GeolocationData;
        }

        public string ObjectID { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CreateDate { get; set; }

        /// <summary>
        /// Unix timestamp
        /// </summary>
        public long CreateDateTimestamp { get; set; }

        public string CreatorName { get; set; }

        public string UpdateDate { get; set; }

        /// <summary>
        /// Unix timestamp
        /// </summary>
        public long UpdateDateTimestamp { get; set; }

        public string WriterName { get; set; }

        public int TemplateId { get; set; }

        public int Level { get; set; }

        public List<string> Path { get; set; }

        public string ContentTypeAlias { get; set; }

        public string Url { get; set; }

        [JsonProperty("_geoloc")]
        public List<GeolocationEntity> GeolocationData { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
