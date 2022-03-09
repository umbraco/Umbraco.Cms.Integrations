using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;


#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
#else
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCommerceShopify")]
    public class ProductsController : UmbracoAuthorizedApiController
    {
        private readonly IShopifyService _apiService;

        public ProductsController(IShopifyService apiService)
        {
            _apiService = apiService;
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
