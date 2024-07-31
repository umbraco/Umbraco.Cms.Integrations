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
    public class GetListByIdsController : ShopifyControllerBase
    {
        public GetListByIdsController(IOptions<ShopifySettings> shopifySettings, IShopifyService shopifyService, IShopifyAuthorizationService shopifyAuthorizationService) : base(shopifySettings, shopifyService, shopifyAuthorizationService)
        {
        }

        [HttpPost("list-by-ids")]
        [ProducesResponseType(typeof(ResponseDto<ProductsListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListByIds([FromBody] RequestDto dto)
        {
            var result = await ShopifyService.GetProductsByIds(dto.Ids.Select(p => p).ToArray());
            return Ok(result);
        }
    }
}
