using System.Configuration;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Services
{
    public class HubspotService: IHubspotService
    {
        private string ClientId => ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthClientId"];

        private string RedirectUrl => ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthRedirectUrl"];

        private string Scopes => ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.OAuthScopes"];

        private const string AuthorizationUrlFormat =
            "https://app-eu1.hubspot.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}";

        public string GetAuthorizationUrl()
        {
            return string.Format(AuthorizationUrlFormat, ClientId, RedirectUrl, Scopes);
        }
    }
}
