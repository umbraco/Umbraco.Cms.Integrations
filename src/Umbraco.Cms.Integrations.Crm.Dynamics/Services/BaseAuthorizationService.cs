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

        public BaseAuthorizationService(DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService)
        {
            DynamicsService = dynamicsService;

            DynamicsConfigurationService = dynamicsConfigurationService;
        }

        
    }
}
