using System;
using System.Net.Http;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public class BaseAuthorizationService
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        protected const string SearchConsoleAuthorizationUrl = "https://accounts.google.com/o/oauth2/auth" +
            "?redirect_uri={0}" +
            "&prompt=consent" +
            "&response_type=code" +
            "&client_id={1}" +
            "&scope={2}" +
            "&access_type=offline";

        protected readonly ITokenService TokenService;

        public BaseAuthorizationService(ITokenService tokenService)
        {
            TokenService = tokenService;
        }
    }
}
