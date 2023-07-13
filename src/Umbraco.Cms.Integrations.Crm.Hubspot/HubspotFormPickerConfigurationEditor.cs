using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.IO;

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    public class HubspotFormPickerConfigurationEditor: ConfigurationEditor<HubspotFormPickerConfiguration>
    {
        public HubspotFormPickerConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
        {
        }
    }
}
