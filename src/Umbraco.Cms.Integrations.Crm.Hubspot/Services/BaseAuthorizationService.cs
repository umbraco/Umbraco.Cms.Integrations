namespace Umbraco.Cms.Integrations.Crm.Hubspot.Services
{
    public class BaseAuthorizationService
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        protected const string HubspotAuthorizationUrl = "https://app-eu1.hubspot.com/oauth/authorize" +
            "?client_id={0}" +
            "&redirect_uri={1}" +
            "&scope={2}";

        protected readonly ITokenService TokenService;

        public BaseAuthorizationService(ITokenService tokenService)
        {
            TokenService = tokenService;
        }

    }
}
