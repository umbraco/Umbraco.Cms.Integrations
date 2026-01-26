using System;
using System.Net.Http;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public class BaseAuthorizationService
    {
        protected readonly IHttpClientFactory HttpClientFactory;

        protected const string SearchConsoleAuthorizationUrl = "https://accounts.google.com/o/oauth2/auth" +
            "?redirect_uri={0}" +
            "&prompt=consent" +
            "&response_type=code" +
            "&client_id={1}" +
            "&scope={2}" +
            "&access_type=offline";

        protected readonly ITokenService TokenService;

        public BaseAuthorizationService(ITokenService tokenService, IHttpClientFactory httpClientFactory)
        {
            TokenService = tokenService;
            HttpClientFactory = httpClientFactory;
        }
    }
}
