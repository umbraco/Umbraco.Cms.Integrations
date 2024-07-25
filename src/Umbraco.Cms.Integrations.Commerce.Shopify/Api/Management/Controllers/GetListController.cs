using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetListController : ShopifyControllerBase
    {
        public GetListController(IOptions<ShopifySettings> shopifySettings, IShopifyService shopifyService, IShopifyAuthorizationService shopifyAuthorizationService) : base(shopifySettings, shopifyService, shopifyAuthorizationService)
        {
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(ResponseDto<ProductsListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList(string? pageInfo)
        {
            var result = await ShopifyService.GetResults(pageInfo);
            result.Skip = 0;
            result.Take = 5;
            return Ok(result);
        }
    }
}
