using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.PIM.Inriver
{
    [DataEditor(
        alias: Constants.PropertyEditorAlias,
        name: "Inriver Entity Picker",
        view: "~/App_Plugins/UmbracoCms.Integrations/PIM/Inriver/views/entitypicker.html",
        Group = Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = "icon-box")]
    public class EntityPickerPropertyEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        public EntityPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory,
            IIOHelper ioHelper,
            IEditorConfigurationParser editorConfigurationParser) 
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;

            _editorConfigurationParser = editorConfigurationParser;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new EntityPickerConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }
}
