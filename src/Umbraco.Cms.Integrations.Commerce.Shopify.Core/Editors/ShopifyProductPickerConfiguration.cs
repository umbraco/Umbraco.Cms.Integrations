//using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models;

//#if NETCOREAPP
//using Umbraco.Cms.Core.PropertyEditors;
//#else
//using Umbraco.Core.PropertyEditors;
//#endif

//namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Editors
//{
//    public class ShopifyProductPickerConfiguration
//    {
//        [ConfigurationField(
//            key: "productPickerSettings", 
//            name: "Settings", 
//            view: "~/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify/views/productPickerSettings.html")]
//        public ProductPickerSettings ProductPickerSettings { get; set; }

//        [ConfigurationField("validationLimit", "Amount", "numberrange", Description = "Set a required range of items selected")]
//        public NumberRange ValidationLimit { get; set; } = new NumberRange();
//    }
//}
