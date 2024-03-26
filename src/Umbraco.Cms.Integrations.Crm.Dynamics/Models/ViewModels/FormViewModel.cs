using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels
{
    public class FormViewModel
    {
        [JsonProperty("iframeEmbedded")]
        public bool IframeEmbedded { get; set; }

        [JsonProperty("formBlockId")]
        public string FormBlockId { get; set; }

        [JsonProperty("containerId")]
        public string ContainerId { get; set; }

        [JsonProperty("containerClass")]
        public string ContainerClass { get; set; }

        [JsonProperty("websiteId")]
        public string WebsiteId { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        public string RawHtml { get; set; }
    }
}
