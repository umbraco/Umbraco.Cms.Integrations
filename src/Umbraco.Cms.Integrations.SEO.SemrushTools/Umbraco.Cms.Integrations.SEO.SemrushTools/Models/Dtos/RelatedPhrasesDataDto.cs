using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos
{
    public class RelatedPhrasesDataDto
    {
        [JsonProperty("columnNames")]
        public string[] ColumnNames { get; set; }

        [JsonProperty("rows")]
        public List<string[]> Rows { get; set; }
    }


}
