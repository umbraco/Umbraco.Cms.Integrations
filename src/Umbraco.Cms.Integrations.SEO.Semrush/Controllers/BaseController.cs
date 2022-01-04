using System.Web;

using Umbraco.Cms.Integrations.SEO.Semrush.Services;
using Umbraco.Web.Editors;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Controllers
{
    public class BaseController: UmbracoAuthorizedJsonController
    {
        protected ISemrushTokenService SemrushTokenService;

        protected ICacheHelper CacheHelper;

        public const int PageSize = 10;

        public const string AuthProxyBaseAddress = "https://localhost:44364/";

        public const string AuthProxyTokenEndpoint = "oauth/v1/token";

        public const string SemrushBaseAddress = "https://oauth.semrush.com/";

        public readonly string SemrushAuthorizationEndpoint 
            = $"{SemrushBaseAddress}oauth2/authorize?ref=0053752252&client_id=umbraco&redirect_uri=%2Foauth2%2Fumbraco%2Fsuccess&response_type=code&scope=user.id,domains.info,url.info,positiontracking.info";

        public readonly string SemrushKeywordsEndpoint =
            SemrushBaseAddress + "api/v1/keywords/{0}?access_token={1}&phrase={2}&database={3}";

        public const string TokenDbKey = "Umbraco.Cms.Integrations.Semrush.TokenDbKey";

        public string SemrushDataSourcesPath = HttpContext.Current.Server.MapPath(
            "/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrushDataSources.json");

        public BaseController(ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper)
        {
            SemrushTokenService = semrushTokenService;

            CacheHelper = cacheHelper;
        }
    }
}
