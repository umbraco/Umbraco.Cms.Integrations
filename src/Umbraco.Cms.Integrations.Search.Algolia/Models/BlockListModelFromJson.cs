using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.Blocks;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models {
    public class BlockListModelFromJson
    {
        public Layout layout { get; set; }
        public List<BlockItemData> contentData { get; set; }
        public List<object> settingsData { get; set; }
    }

    public class Layout
    {
        [JsonProperty("Umbraco.BlockList")]
        public List<UmbracoBlockList> UmbracoBlockList { get; set; }
    }
   
    public class UmbracoBlockList
    {
        public string contentUdi { get; set; }
    }
}
