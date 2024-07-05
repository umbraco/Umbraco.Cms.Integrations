using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Api.Management.Controllers;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.New.Api.Management.Controllers
{
    public class GetAllOAuthController : ShopifyControllerBase
    {
        protected GetAllOAuthController(IOptions<ShopifySettings> shopifySettings) : base(shopifySettings)
        {
        }
    }
}
