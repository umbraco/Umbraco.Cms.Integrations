using System;
using System.Net.Http;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class BaseAuthorizationService
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        protected readonly DynamicsService DynamicsService;

        protected readonly DynamicsConfigurationService DynamicsConfigurationService;

        protected const string DynamicsAuthorizationUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize" +
            "?client_id={0}" +
            "&response_type=code" +
            "&redirect_uri={1}" +
            "&response_mode=query" +
            "&scope={2}";

        public BaseAuthorizationService(DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService)
        {
            DynamicsService = dynamicsService;

            DynamicsConfigurationService = dynamicsConfigurationService;
        }


    }
}
