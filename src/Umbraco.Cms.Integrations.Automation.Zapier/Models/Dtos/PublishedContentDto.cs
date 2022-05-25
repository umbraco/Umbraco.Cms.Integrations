using System;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    [Obsolete("Used only by Umbraco Zapier App v1.0.0")]
    public class PublishedContentDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("publishDate")]
        public string PublishDate { get; set; }
    }
}
