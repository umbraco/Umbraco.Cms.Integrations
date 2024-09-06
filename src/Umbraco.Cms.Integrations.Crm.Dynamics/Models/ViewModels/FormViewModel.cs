
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels
{
    public class FormViewModel
    {
        [JsonPropertyName("iframeEmbedded")]
        public bool IframeEmbedded { get; set; }

        [JsonPropertyName("formBlockId")]
        public string FormBlockId { get; set; }

        [JsonPropertyName("containerId")]
        public string ContainerId { get; set; }

        [JsonPropertyName("containerClass")]
        public string ContainerClass { get; set; }

        [JsonPropertyName("websiteId")]
        public string WebsiteId { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        public DynamicsModule Module { get; set; }

        public string Html { get; set; }

        public string StandaloneUrl
        {
            get
            {
                const string dataCachedFormUrlKey = "data-cached-form-url=";

                if (string.IsNullOrEmpty(Html))
                {
                    return string.Empty;
                }

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
