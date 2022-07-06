using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Configuration
{
    public class DynamicsSettings
    {
        public DynamicsSettings() { }

        public DynamicsSettings(NameValueCollection appSettings)
        {
            InstanceUrl = appSettings[Constants.UmbracoCmsIntegrationsCrmDynamicsInstanceUrlKey];
        }

        public string InstanceUrl { get; set; }
    }
}
