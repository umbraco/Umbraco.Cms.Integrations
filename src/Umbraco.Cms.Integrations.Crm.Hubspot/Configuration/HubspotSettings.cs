using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Configuration
{
    public class HubspotSettings
    {
        public HubspotSettings()
        {
            
        }

        public HubspotSettings(NameValueCollection appSettings)
        {
            ApiKey = appSettings[Constants.UmbracoCmsIntegrationsCrmHubspotApiKey];
            Region = appSettings[Constants.UmbracoCmsIntegrationsCrmHubspotRegion];
        }

        public string ApiKey { get; set; }

        public string Region { get; set; }
    }
}
