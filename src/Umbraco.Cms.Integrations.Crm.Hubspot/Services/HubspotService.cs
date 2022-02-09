using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Services
{
    public class HubspotService: IHubspotService
    {
        private readonly IAppSettings _appSettings;

        public HubspotService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        private string ClientId => _appSettings[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotOAuthClientId];

        private string RedirectUrl =>
            _appSettings[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotOAuthRedirectUrl];

        private string AuthorizationBaseUrl =>
            _appSettings[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotOAuthAuthorizationBaseUrl];

        private string Scopes => _appSettings[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotOAuthScopes];

        private const string AuthorizationUrlFormat = "{0}?client_id={1}&redirect_uri={2}&scope={3}";

        public string GetAuthorizationUrl()
        {
            return string.Format(AuthorizationUrlFormat, AuthorizationBaseUrl, ClientId, RedirectUrl, Scopes);
        }
    }
}
