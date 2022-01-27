using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class DataSourceDto
    {
        public DataSourceDto()
        {
            Items = Enumerable.Empty<DataSourceItemDto>();
        }
        public IEnumerable<DataSourceItemDto> Items { get; set; }
    }

    public class DataSourceItemDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("researchTypes")]
        public string ResearchTypes { get; set; }

        [JsonProperty("googleSearchDomain")]
        public string GoogleSearchDomain { get; set; }
    }
}
