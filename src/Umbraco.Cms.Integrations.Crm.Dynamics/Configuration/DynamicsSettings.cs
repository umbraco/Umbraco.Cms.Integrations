using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Configuration
{
    public class DynamicsSettings
    {
        public DynamicsSettings() { }

        public DynamicsSettings(NameValueCollection appSettings)
        {
            InstanceUrl = appSettings[Constants.UmbracoCmsIntegrationsCrmDynamicsInstanceUrlKey];

            InstanceWebApiUrl = appSettings[Constants.UmbracoCmsIntegrationsCrmDynamicsInstanceWebApiUrlKey];
        }

        public string InstanceUrl { get; set; }

        public string InstanceWebApiUrl { get; set; }
    }
}
