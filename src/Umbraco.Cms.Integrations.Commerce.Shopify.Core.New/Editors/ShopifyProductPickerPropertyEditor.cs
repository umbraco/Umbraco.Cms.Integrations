//#if NETCOREAPP
//using Umbraco.Cms.Core.IO;
//using Umbraco.Cms.Core.PropertyEditors;
//#else
//using Umbraco.Core.Logging;
//using Umbraco.Core.PropertyEditors;
//#endif

//namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
//{
//    [DataEditor(
//        alias: "Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker",
//        name: "Shopify Product Picker",
//        view: "~/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify/views/productPicker.html",
//        Group = "Pickers",
//        Icon = "icon-shopping-basket-alt")]
//    public class ShopifyProductPickerPropertyEditor: DataEditor
//    {
//#if NETCOREAPP
//        private IIOHelper _ioHelper;

//        public ShopifyProductPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory)
//        {
//            _ioHelper = ioHelper;
//        }

//        protected override IConfigurationEditor CreateConfigurationEditor()
//        {
//            return new ShopifyProductPickerConfigurationEditor(_ioHelper);
//        }
//#else
//        public ShopifyProductPickerPropertyEditor(ILogger logger) : base(logger)
//        {
//        }

//        protected override IConfigurationEditor CreateConfigurationEditor()
//        {
//            return new ShopifyProductPickerConfigurationEditor();
//        }
//#endif
//    }
//}
