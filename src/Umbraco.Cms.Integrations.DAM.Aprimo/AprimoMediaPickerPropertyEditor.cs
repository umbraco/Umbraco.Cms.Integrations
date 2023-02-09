using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.DAM.Aprimo
{
    [DataEditor(
        alias: Constants.PropertyEditorAlias,
        name: "Aprimo Media Picker",
        view: "~/App_Plugins/UmbracoCms.Integrations/DAM/Aprimo/views/mediapicker.html",
        Group = Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = "icon-umb-media")]
    public class AprimoMediaPickerPropertyEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;
        
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        public AprimoMediaPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, 
            IIOHelper ioHelper, 
            IEditorConfigurationParser editorConfigurationParser) 
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;   

            _editorConfigurationParser = editorConfigurationParser;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => 
            new AprimoMediaPickerConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }
}
