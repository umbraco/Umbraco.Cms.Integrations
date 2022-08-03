
#if NETCOREAPP
using Umbraco.Cms.Core.PropertyEditors;
#else
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics
{
    public class DynamicsFormPickerConfiguration
    {
        [ConfigurationField("configuration", name: "Configuration", view: "~/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/views/configuration.html",
            Description = "Connect with your Microsoft account.")]
        public string IsConnected { get; set; }
    }
}
