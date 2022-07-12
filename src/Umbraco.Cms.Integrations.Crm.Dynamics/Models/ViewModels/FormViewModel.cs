using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels
{
    public class FormViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("embedCode")]
        public string EmbedCode { get; set; }
    }
}
