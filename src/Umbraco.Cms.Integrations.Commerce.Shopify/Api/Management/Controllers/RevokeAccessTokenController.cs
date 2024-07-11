﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Api.Management.Controllers;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class RevokeAccessTokenController : ShopifyControllerBase
    {
        public RevokeAccessTokenController(IOptions<ShopifySettings> shopifySettings, IShopifyService shopifyService, IShopifyAuthorizationService shopifyAuthorizationService) : base(shopifySettings, shopifyService, shopifyAuthorizationService)
        {
        }

        [HttpPost("revoke-access-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult RevokeAccessToken()
        {
            ShopifyService.RevokeAccessToken();
            return Ok();
        }
    }
}
