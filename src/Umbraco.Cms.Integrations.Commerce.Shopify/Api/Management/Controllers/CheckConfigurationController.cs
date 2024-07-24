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
using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class CheckConfigurationController : ShopifyControllerBase
    {
        private readonly ShopifyOAuthSettings _oauthSettings;
        public CheckConfigurationController(IOptions<ShopifySettings> shopifySettings, IShopifyService shopifyService, IShopifyAuthorizationService shopifyAuthorizationService, IOptions<ShopifyOAuthSettings> oauthSettings) : base(shopifySettings, shopifyService, shopifyAuthorizationService)
        {
            _oauthSettings = oauthSettings.Value;
        }

        [HttpGet("check-configuration")]
        [ProducesResponseType(typeof(EditorSettings), StatusCodes.Status200OK)]
        public IActionResult CheckConfiguration()
        {
            var settings = ShopifyService.GetApiConfiguration();

            return Ok(settings);
        }
    }
}
