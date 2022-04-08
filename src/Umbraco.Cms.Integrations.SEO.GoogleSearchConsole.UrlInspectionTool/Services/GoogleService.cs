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

        private const string ClientId = "849175818654-0jtc4c8baoo58d3ruhbkghao425ejrvf.apps.googleusercontent.com";

        public string[] Scopes = new[]
        {
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile",
            "https://www.googleapis.com/auth/webmasters",
            "https://www.googleapis.com/auth/webmasters.readonly"
        };

        public string SearchConsoleAuthorizationEndpoint =
            "https://accounts.google.com/o/oauth2/auth?response_type=code" +
            "&redirect_uri={0}&scope={1}&client_id={2}";

        public override string GetClientId() => ClientId;

        public override string GetAuthorizationUrl() =>
            string.Format(SearchConsoleAuthorizationEndpoint, $"{AuthProxyBaseAddress}{AuthProxyServiceAuthEndpoint}", string.Join(" ", Scopes), ClientId);

        public override string GetAuthProxyTokenEndpoint() => $"{AuthProxyBaseAddress}{AuthProxyTokenEndpoint}";
    }
}
