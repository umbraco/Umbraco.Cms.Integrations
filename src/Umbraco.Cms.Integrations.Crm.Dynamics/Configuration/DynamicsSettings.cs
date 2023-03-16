using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Configuration
{
    public class DynamicsSettings
    {
        public DynamicsSettings() { }

        public DynamicsSettings(NameValueCollection appSettings)
        {
            HostUrl = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsHostUrlKey];

            ApiPath = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmDynamicsApiPathKey];
        }

        public string HostUrl { get; set; }

        public string ApiPath { get; set; }

        public bool UseUmbracoAuthorization { get; set; }
    }
}
