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

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.New.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetAllController : ShopifyControllerBase
    {
        protected GetAllController(IOptions<ShopifySettings> shopifySettings) : base(shopifySettings)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task GetAll()
        {

        }
    }
}
