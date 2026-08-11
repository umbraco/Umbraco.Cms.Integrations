using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
{
    // ValueType must be JSON: the editor posts an array of product ids, and without it the default
    // string storage type calls ToString() on the collection, writing "System.Collections.Generic.List`1[System.Int64]"
    // into the database instead of the ids.
    [DataEditor(
        Constants.PropertyEditors.ProductPickerAlias,
        ValueType = ValueTypes.Json,
        ValueEditorIsReusable = true)]
    public class ShopifyProductPickerPropertyEditor : DataEditor
    {
        public ShopifyProductPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
        }
    }
}
