using System;
using System.Net.Http;

#if NETCOREAPP
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#else
using Umbraco.Core.Logging;
#endif

namespace Umbraco.Cms.Integrations.Shared.Services
{
    public abstract class BaseService
    {
        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/";  // for local testing: "https://localhost:44364/";
        public const string OAuthProxyEndpoint = "{0}oauth/v1/token";

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private static readonly HttpClient Client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => Client;

#if NETCOREAPP
        public readonly ILogger<BaseService> UmbCoreLogger;
#else
        public readonly ILogger UmbCoreLogger;
#endif

        public readonly ITokenService TokenService;

#if NETCOREAPP
        public BaseService(ILogger<BaseService> logger, ITokenService tokenService)
        {
            UmbCoreLogger = logger;

            TokenService = tokenService;
        }
#else
        public BaseService(ILogger logger, ITokenService tokenService)
        {
            UmbCoreLogger = logger;

            TokenService = tokenService;
        }
#endif
    }
}
