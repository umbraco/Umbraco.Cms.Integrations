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
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Api.Management.Controllers;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.New.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetListByIdsController : ShopifyControllerBase
    {
        public GetListByIdsController(IOptions<ShopifySettings> shopifySettings, IShopifyService shopifyService, IShopifyAuthorizationService shopifyAuthorizationService) : base(shopifySettings, shopifyService, shopifyAuthorizationService)
        {
        }

        [HttpPost("get-list-by-ids")]
        [ProducesResponseType(typeof(ResponseDto<ProductsListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListByIds([FromBody] RequestDto dto)
        {
            var result = await ShopifyService.GetProductsByIds(dto.Ids.Select(p => (long.Parse(p))).ToArray());
            return Ok(result);
        }
    }
}
