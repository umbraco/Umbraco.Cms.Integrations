using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using UmbracoConstants = Umbraco.Core.Constants;
namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Editors
{
    [DataEditor(
        Constants.PropertyEditors.PickerAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        "CommerceTools Picker",
        Constants.AppPluginFolderPath + "/PropertyEditors/CommerceToolsPicker.html",
        ValueType = ValueTypes.String,
        Group = UmbracoConstants.PropertyEditors.Groups.Pickers)]
    public class CommerceToolsPicker : DataEditor
    {

        public CommerceToolsPicker(ILogger logger) : base(logger)
        {
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CommerceToolsPickerConfigurationEditor();
    }
}
