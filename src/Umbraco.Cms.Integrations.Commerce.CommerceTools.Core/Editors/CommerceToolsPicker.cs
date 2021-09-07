#if NETCOREAPP
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using UmbracoConstants = Umbraco.Cms.Core.Constants;
#else
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using UmbracoConstants = Umbraco.Core.Constants;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Editors
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

#if NETCOREAPP
        private IIOHelper _ioHelper;

        public CommerceToolsPicker(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CommerceToolsPickerConfigurationEditor(_ioHelper);
#else
        public CommerceToolsPicker(ILogger logger) : base(logger)
        {
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CommerceToolsPickerConfigurationEditor();
#endif
    }
}
