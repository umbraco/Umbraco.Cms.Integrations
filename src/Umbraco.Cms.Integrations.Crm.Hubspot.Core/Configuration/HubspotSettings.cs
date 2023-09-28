using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration
{
    public class HubspotSettings
    {
        public HubspotSettings()
        {
            
        }

        public HubspotSettings(NameValueCollection appSettings)
        {
            ApiKey = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotApiKey];
            Region = appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotRegion];
            UseUmbracoAuthorization = 
                bool.TryParse(appSettings[Constants.Configuration.UmbracoCmsIntegrationsCrmHubspotUseUmbracoAuthorizationKey], out var key)
                    ? key
                    : true;
        }

        public string ApiKey { get; set; }

        public string Region { get; set; }

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
