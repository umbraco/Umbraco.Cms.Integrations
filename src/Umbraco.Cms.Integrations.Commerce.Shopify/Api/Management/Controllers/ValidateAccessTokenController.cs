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
    public class ValidateAccessTokenController : ShopifyControllerBase
    {
        public ValidateAccessTokenController(IOptions<ShopifySettings> shopifySettings, IShopifyService shopifyService, IShopifyAuthorizationService shopifyAuthorizationService) : base(shopifySettings, shopifyService, shopifyAuthorizationService)
        {
        }

        [HttpGet("validate-access-token")]
        [ProducesResponseType(typeof(ResponseDto<ProductsListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateAccessToken()
        {
            var accessToken = await ShopifyService.ValidateAccessToken();
            return Ok(accessToken);
        }
    }
}
