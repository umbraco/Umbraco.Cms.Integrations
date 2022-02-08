using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models
{
    public class ProductPickerSettings
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
