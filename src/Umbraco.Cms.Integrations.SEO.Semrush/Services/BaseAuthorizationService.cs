namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public class BaseAuthorizationService
    {
        protected const string SemrushAuthorizationUrl = "https://oauth.semrush.com/oauth2/authorize" +
            "?ref={0}" +
            "&client_id={1}" +
            "&redirect_uri={2}" +
            "&response_type=code" +
            "&scope={3}";

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        protected readonly TokenBuilder TokenBuilder;

        protected readonly ISemrushTokenService SemrushTokenService;

        public BaseAuthorizationService(TokenBuilder tokenBuilder, ISemrushTokenService semrushTokenService)
        {
            TokenBuilder = tokenBuilder;

            SemrushTokenService = semrushTokenService;
        }
    }
}
