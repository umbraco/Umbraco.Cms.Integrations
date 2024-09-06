namespace Umbraco.Cms.Integrations.Crm.Dynamics.Configuration
{
    public class DynamicsSettings
    {
        public string HostUrl { get; set; } = string.Empty;

        public string ApiPath { get; set; } = string.Empty;

        public bool UseUmbracoAuthorization { get; set; } = true;
    }
}
