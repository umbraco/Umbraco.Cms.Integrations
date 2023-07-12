
namespace Umbraco.Cms.Integrations.Crm.Hubspot.Configuration
{
    public class HubspotSettings
    {
        public string ApiKey { get; set; }

        public string Region { get; set; }

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
