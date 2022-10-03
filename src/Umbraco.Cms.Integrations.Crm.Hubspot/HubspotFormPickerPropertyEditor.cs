#if NETCOREAPP
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
#else
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    [DataEditor(
        alias: Constants.PropertyEditorAlias,
        name: "HubSpot Form Picker",
        view: "~/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/views/formpicker.html",
        Group = Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = "icon-handshake"
        )]
    public class HubspotFormPickerPropertyEditor: DataEditor
    {
#if NETCOREAPP

        private IIOHelper _ioHelper;

        public HubspotFormPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new HubspotFormPickerConfigurationEditor(_ioHelper);
        }
#else
        public HubspotFormPickerPropertyEditor(ILogger logger) : base(logger)
        {
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new HubspotFormPickerConfigurationEditor();
        }
#endif
    }
}
