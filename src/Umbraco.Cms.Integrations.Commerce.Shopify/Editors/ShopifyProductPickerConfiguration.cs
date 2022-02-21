using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
{
    public class ShopifyProductPickerConfiguration
    {
        [ConfigurationField(
            key: "productPickerSettings", 
            name: "Settings", 
            view: "~/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify/views/productPickerSettings.html")]
        public ProductPickerSettings ProductPickerSettings { get; set; }
    }
}
