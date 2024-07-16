using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Api.Management.Controllers;
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

        [HttpGet("get-list")]
        [ProducesResponseType(typeof(ResponseDto<ProductsListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList(string? pageInfo)
        {
            var result = await ShopifyService.GetResults(pageInfo);
            return Ok(result);
        }
    }
}
