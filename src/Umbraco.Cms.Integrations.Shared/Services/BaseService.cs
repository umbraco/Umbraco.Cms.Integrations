using System;
using System.Net.Http;

using Umbraco.Cms.Integrations.Shared.Configuration;
using Umbraco.Core.Logging;

namespace Umbraco.Cms.Integrations.Shared.Services
{
    public abstract class BaseService
    {
        public const string OAuthProxyBaseUrl = "https://localhost:44364/";
        public const string OAuthProxyEndpoint = "{0}oauth/v1/token";

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private static readonly HttpClient Client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => Client;

        public readonly ILogger UmbCoreLogger;

        public readonly IAppSettings AppSettings;

        public readonly ITokenService TokenService;

        public BaseService(ILogger logger, IAppSettings appSettings, ITokenService tokenService)
        {
            UmbCoreLogger = logger;

            AppSettings = appSettings;

            TokenService = tokenService;
        }
    }
}
