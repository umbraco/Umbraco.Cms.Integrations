using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign
{
    [DataEditor(
        alias: Constants.PropertyEditorAlias,
        name: "ActiveCampaign Form Picker",
        view: "~/App_Plugins/UmbracoCms.Integrations/Crm/ActiveCampaign/views/formpicker.html",
        Group = Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = "icon-activecampaign")]
    public class ActiveCampaignFormPickerPropertyEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;

        private readonly IEditorConfigurationParser _editorConfigurationParser;

        public ActiveCampaignFormPickerPropertyEditor(
            IDataValueEditorFactory dataValueEditorFactory,
            IIOHelper ioHelper,
            IEditorConfigurationParser editorConfigurationParser)
            : base(dataValueEditorFactory) 
        {
            _ioHelper = ioHelper;

            _editorConfigurationParser = editorConfigurationParser;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => 
            new ActiveCampaignFormPickerConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }
}
