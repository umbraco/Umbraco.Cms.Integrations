#if NETCOREAPP
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.IO;
#else
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    public class HubspotFormPickerConfigurationEditor: ConfigurationEditor<HubspotFormPickerConfiguration>
    {
#if NETCOREAPP
        public HubspotFormPickerConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
        {
        }
#endif
    }
}
