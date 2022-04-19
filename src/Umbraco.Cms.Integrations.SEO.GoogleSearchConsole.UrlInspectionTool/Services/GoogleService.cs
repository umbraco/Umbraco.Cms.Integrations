using System.Collections.Generic;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public class GoogleService : BaseAuthService
    {
        /// <summary>
        /// local testing: https://localhost:44364/
        /// deployed version: https://hubspot-forms-auth.umbraco.com/
        /// </summary>
        private const string AuthProxyBaseAddress = "https://localhost:44364/";

        public const string AuthProxyServiceAuthEndpoint = "oauth/google";

        public const string AuthProxyTokenEndpoint = "oauth/v1/token";

        public KeyValuePair<string, string> ServiceHeaderKey = new KeyValuePair<string, string>("service_name", "Google");

        public string TokenDbKey = "Umbraco.Cms.Integrations.GoogleSearchConsole.URLInspectionToolTokenDbKey";

        public string RefreshTokenDbKey =
            "Umbraco.Cms.Integrations.GoogleSearchConsole.URLInspectionToolRefreshTokenDbKey";

        private const string ClientId = "849175818654-0jtc4c8baoo58d3ruhbkghao425ejrvf.apps.googleusercontent.com";

        public string[] Scopes = new[]
        {
            "https://www.googleapis.com/auth/webmasters",
            "https://www.googleapis.com/auth/webmasters.readonly"
        };

        public string SearchConsoleAuthorizationEndpoint =
            "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&prompt=consent&response_type=code&client_id={1}&scope={2}&access_type=offline";

        public override string GetClientId() => ClientId;

        public override string GetAuthorizationUrl() =>
            string.Format(SearchConsoleAuthorizationEndpoint, $"{AuthProxyBaseAddress}{AuthProxyServiceAuthEndpoint}", ClientId, string.Join(" ", Scopes));

        public override string GetRedirectUrl() => $"{AuthProxyBaseAddress}{AuthProxyServiceAuthEndpoint}";

        public override string GetAuthProxyTokenEndpoint() => $"{AuthProxyBaseAddress}{AuthProxyTokenEndpoint}";

        public string GetSearchConsoleInpectionUrl() => "https://searchconsole.googleapis.com/v1/urlInspection/index:inspect";
    }
}
