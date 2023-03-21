using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.DAM.Aprimo
{
    public class AprimoMediaPickerConfigurationEditor : ConfigurationEditor<AprimoMediaPickerConfiguration>
    {
        public AprimoMediaPickerConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) 
            : base(ioHelper, editorConfigurationParser)
        {
        }
    }
}
