using System.Collections.Generic;
using System.Web;

using Umbraco.Cms.Integrations.SEO.Semrush.Services;
using Umbraco.Web.Editors;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Controllers
{
    public class BaseController: UmbracoAuthorizedJsonController
    {
        protected ISemrushTokenService SemrushTokenService;

        protected ICacheHelper CacheHelper;

        protected TokenBuilder TokenBuilder;

        public string SemrushDataSourcesPath = HttpContext.Current.Server.MapPath(
            "/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrushDataSources.json");

        public BaseController(ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper, TokenBuilder tokenBuilder)
        {
            SemrushTokenService = semrushTokenService;

            CacheHelper = cacheHelper;

            TokenBuilder = tokenBuilder;
        }
    }
}
