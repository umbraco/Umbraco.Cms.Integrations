using System;
using System.Net.Http;

using Umbraco.Cms.Integrations.Shared.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Web.WebApi;

namespace Umbraco.Cms.Integrations.Shared.Controllers
{
    public class BaseAuthorizedApiController : UmbracoAuthorizedApiController
    {
        public readonly ILogger ApiLogger;

        public readonly IAppSettings AppSettings;

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        public static readonly HttpClient Client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => Client;

        public BaseAuthorizedApiController(ILogger logger, IAppSettings appSettings)
        {
            ApiLogger = logger;

            AppSettings = appSettings;
        }
    }
}
