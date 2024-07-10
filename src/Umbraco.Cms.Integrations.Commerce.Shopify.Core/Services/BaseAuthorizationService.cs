using System;
using System.Net.Http;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Services
{
    public class BaseAuthorizationService
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        protected readonly ITokenService TokenService;

        protected const string ShopifyAuthorizationUrl = "https://{0}.myshopify.com/admin/oauth/authorize" +
            "?client_id={1}" +
            "&redirect_uri={2}" +
            "&scope=read_products" +
            "&grant_options[]=value";

        public BaseAuthorizationService(ITokenService tokenService)
        {
            TokenService = tokenService;
        }
    }
}
