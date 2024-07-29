using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
{
    [DataEditor(
        "Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker",
        ValueEditorIsReusable = true)]
    public class ShopifyProductPickerPropertyEditor : DataEditor
    {
        public ShopifyProductPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
        }
    }
}
