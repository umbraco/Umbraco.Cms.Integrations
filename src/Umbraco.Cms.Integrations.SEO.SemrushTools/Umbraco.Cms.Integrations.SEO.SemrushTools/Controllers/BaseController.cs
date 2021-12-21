using Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.SemrushTools.Services;
using Umbraco.Web.Editors;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Controllers
{
    public class BaseController: UmbracoAuthorizedJsonController
    {
        protected ISemrushService<TokenDto> SemrushService;

        protected ISemrushCachingService<RelatedPhrasesDto> CachingService;

        public const string SemrushBaseAddress = "https://oauth.semrush.com/";

        public const string BaseAuthorizationHubAddress = "https://localhost:44358/authorizationhubapi/semrush/";

        public readonly string SemrushAuthorizationEndpoint 
            = $"{SemrushBaseAddress}oauth2/authorize?ref=0053752252&client_id=umbraco&redirect_uri=%2Foauth2%2Fumbraco%2Fsuccess&response_type=code&scope=user.id,domains.info,url.info,positiontracking.info";

        public readonly string KeywordsEndpoint =
            SemrushBaseAddress + "api/v1/keywords/phrase_related?access_token={0}&phrase={1}&database=us";

        public BaseController(ISemrushService<TokenDto> semrushService, ISemrushCachingService<RelatedPhrasesDto> cachingService)
        {
            SemrushService = semrushService;

            CachingService = cachingService;
        }
    }
}
