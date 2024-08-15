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

        public DynamicsModule Module { get; set; }

        public string Html { get; set; }

        public string StandaloneUrl
        {
            get
            {
                const string dataCachedFormUrlKey = "data-cached-form-url=";

                var dataCachedFromUrl = Html.Split(' ').FirstOrDefault(p => p.Contains("data-cached-form-url"));
                if (string.IsNullOrEmpty(dataCachedFromUrl))
                {
                    return string.Empty;
                }

                return dataCachedFromUrl
                    .Replace("'", "")
                    .Substring(dataCachedFromUrl.IndexOf(dataCachedFormUrlKey) + dataCachedFormUrlKey.Length)
                    .Replace("forms", "standaloneforms");
            }
        }
    }
}
