using System.Collections.Generic;
using System.Linq;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos
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
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
