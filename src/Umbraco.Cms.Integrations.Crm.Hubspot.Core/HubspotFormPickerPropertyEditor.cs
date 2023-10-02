using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core
{
    [DataEditor(
        alias: Constants.PropertyEditorAlias,
        name: "HubSpot Form Picker",
        view: "~/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/views/formpicker.html",
        Group = Cms.Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = "icon-handshake"
        )]
    public class HubspotFormPickerPropertyEditor: DataEditor
    {
        private IIOHelper _ioHelper;

        public HubspotFormPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new HubspotFormPickerConfigurationEditor(_ioHelper);
        }
    }
}
