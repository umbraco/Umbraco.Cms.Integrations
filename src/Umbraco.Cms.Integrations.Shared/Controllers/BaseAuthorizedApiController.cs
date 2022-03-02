using System;
using System.Net.Http;

using Umbraco.Cms.Integrations.Shared.Services;

#if NETCOREAPP
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.Extensions.Logging;
#else
using Umbraco.Web.WebApi;
using Umbraco.Core.Logging;
#endif

namespace Umbraco.Cms.Integrations.Shared.Controllers
{
    public class BaseAuthorizedApiController : UmbracoAuthorizedApiController
    {
#if NETCOREAPP
        public readonly ILogger<BaseAuthorizedApiController> ApiLogger;
#else
        public readonly ILogger ApiLogger;
#endif
        
        public readonly ITokenService TokenService;

        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/";  // for local testing: "https://localhost:44364/"; 
        public const string OAuthProxyEndpoint = "{0}oauth/v1/token";

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        public static readonly HttpClient Client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => Client;

#if NETCOREAPP
        public BaseAuthorizedApiController(ILogger<BaseAuthorizedApiController> logger, ITokenService tokenService)
        {
            ApiLogger = logger;

            TokenService = tokenService;
        }
#else
        public BaseAuthorizedApiController(ILogger logger, ITokenService tokenService)
        {
            ApiLogger = logger;

            TokenService = tokenService;
        }
#endif
    }
}
