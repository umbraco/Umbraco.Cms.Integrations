using System.Threading.Tasks;
using System.Web.Http;

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

        public ProductsController(IApiService<ProductsListDto> apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public EditorSettings CheckConfiguration() => _apiService.GetApiConfiguration();

        [HttpGet]
        public string GetAuthorizationUrl() => _apiService.GetAuthorizationUrl();

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto) => await _apiService.GetAccessToken(authRequestDto);


        public async Task<ResponseDto<ProductsListDto>> GetList() => await _apiService.GetResults();


    }
}
