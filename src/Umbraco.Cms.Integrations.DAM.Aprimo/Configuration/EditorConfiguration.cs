using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Configuration
{
    public class EditorConfiguration
    {
        [JsonProperty("useContentSelector")]
        public bool UseContentSelector { get; set; }
    }
}
