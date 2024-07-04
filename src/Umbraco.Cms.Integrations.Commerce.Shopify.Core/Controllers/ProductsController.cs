using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;

using static Umbraco.Cms.Integrations.Commerce.Shopify.ShopifyComposer;
using System.Linq;


#if NETCOREAPP
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
#else
using System.Configuration;
using System.Web.Http;

using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCommerceShopify")]
    public class ProductsController : UmbracoAuthorizedApiController
    {
        private readonly ShopifySettings _settings;

        private readonly IShopifyAuthorizationService _authorizationService;

        private readonly IShopifyService _apiService;

#if NETCOREAPP
        public ProductsController(IOptions<ShopifySettings> options, IShopifyService apiService, AuthorizationImplementationFactory authorizationImplementationFactory)
#else
        public ProductsController(IShopifyService apiService, AuthorizationImplementationFactory authorizationImplementationFactory)
#endif
        {
#if NETCOREAPP
            _settings = options.Value;
#else
            _settings = new ShopifySettings(ConfigurationManager.AppSettings);
#endif
            _apiService = apiService;
            _authorizationService = authorizationImplementationFactory(_settings.UseUmbracoAuthorization);
        }

        [HttpGet]
        public EditorSettings CheckConfiguration() => _apiService.GetApiConfiguration();

        [HttpGet]
        public string GetAuthorizationUrl() => _authorizationService.GetAuthorizationUrl();

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto) => 
            await _authorizationService.GetAccessTokenAsync(authRequestDto.Code);

        [HttpGet]
        public async Task<ResponseDto<ProductsListDto>> ValidateAccessToken() => await _apiService.ValidateAccessToken();

        [HttpPost]
        public void RevokeAccessToken() => _apiService.RevokeAccessToken();

        public async Task<ResponseDto<ProductsListDto>> GetList(string pageInfo) => await _apiService.GetResults(pageInfo);

        public async Task<ResponseDto<ProductsListDto>> GetListByIds([FromBody] RequestDto dto) => 
            await _apiService.GetProductsByIds(dto.Ids.Select(p => (long.Parse(p))).ToArray());

        [HttpGet]
        public async Task<int> GetTotalPages()
        {
            var productsCount = await _apiService.GetCount();

            return productsCount / Constants.DEFAULT_PAGE_SIZE + 1;
        }
    }
}
