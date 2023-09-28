using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core
{
    [DataEditor(
        alias: Constants.PropertyEditorAlias,
        name: "HubSpot Form Picker",
        view: "~/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/views/formpicker.html",
        Group = Umbraco.Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = "icon-handshake"
        )]
    public class HubspotFormPickerPropertyEditor: DataEditor
    {
        public HubspotFormPickerPropertyEditor(ILogger logger) : base(logger)
        {
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new HubspotFormPickerConfigurationEditor();
        }
    }
}
