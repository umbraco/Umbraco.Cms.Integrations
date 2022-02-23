using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Models;
using Umbraco.Web.Mvc;
using Umbraco.Cms.Integrations.Shared.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Services;
using Umbraco.Web.WebApi;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCommerceShopify")]
    public class ProductsController : UmbracoAuthorizedApiController
    {
        private readonly IApiService<ProductsListDto> _apiService;

        private readonly ICacheHelper _cacheHelper;

        public ProductsController(IApiService<ProductsListDto> apiService, ICacheHelper cacheHelper)
        {
            _apiService = apiService;

            _cacheHelper = cacheHelper;
        }

        [HttpGet]
        public EditorSettings CheckConfiguration() => _apiService.GetApiConfiguration();

        [HttpGet]
        public string GetAuthorizationUrl() => _apiService.GetAuthorizationUrl();

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto) => await _apiService.GetAccessToken(authRequestDto);

        [HttpGet]
        public async Task<ResponseDto<ProductsListDto>> ValidateAccessToken() => await _apiService.ValidateAccessToken();

        [HttpPost]
        public void RevokeAccessToken() => _apiService.RevokeAccessToken();

        public async Task<ResponseDto<ProductsListDto>> GetList() => await _apiService.GetResults();
    }
}
