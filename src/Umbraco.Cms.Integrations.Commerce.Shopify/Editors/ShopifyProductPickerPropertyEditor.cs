using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
{
    [DataEditor(
        alias: "Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker",
        name: "Shopify Product Picker",
        view: "~/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify/views/productPicker.html",
        Group = "Pickers",
        Icon = "icon-shopping-basket-alt")]
    public class ShopifyProductPickerPropertyEditor: DataEditor
    {
        public ShopifyProductPickerPropertyEditor(ILogger logger) : base(logger)
        {
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new ShopifyProductPickerConfigurationEditor();
        }
    }
}
