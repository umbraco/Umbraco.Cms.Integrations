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

        protected readonly IHttpClientFactory HttpClientFactory;

        protected readonly TokenBuilder TokenBuilder;

        protected readonly ISemrushTokenService SemrushTokenService;

        public BaseAuthorizationService(TokenBuilder tokenBuilder, ISemrushTokenService semrushTokenService, IHttpClientFactory httpClientFactory)
        {
            TokenBuilder = tokenBuilder;
            SemrushTokenService = semrushTokenService;
            HttpClientFactory = httpClientFactory;
        }
    }
}
