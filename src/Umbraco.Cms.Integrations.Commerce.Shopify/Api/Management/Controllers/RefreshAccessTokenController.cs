using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.Shopify;
using Umbraco.Cms.Integrations.Commerce.Shopify.Api.Management.Controllers;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class RefreshAccessTokenController : ShopifyControllerBase
    {
        public RefreshAccessTokenController(IOptions<ShopifySettings> shopifySettings, IShopifyService shopifyService, IShopifyAuthorizationService shopifyAuthorizationService) : base(shopifySettings, shopifyService, shopifyAuthorizationService)
        {
        }

        [HttpPost("refresh-access-token")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var response = await ShopifyAuthorizationService.RefreshAccessTokenAsync();
            return Ok(response);
        }
    }
}
