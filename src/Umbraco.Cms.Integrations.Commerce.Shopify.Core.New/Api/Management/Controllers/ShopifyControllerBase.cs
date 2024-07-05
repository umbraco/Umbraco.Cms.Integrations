using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;
using System.Linq;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}/shopify")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public abstract class ShopifyControllerBase : Controller
    {
        protected const string ShopifyApiEndpoint = "https://admin.shopify.com";
        protected ShopifySettings ShopifySettings;

        protected ShopifyControllerBase(IOptions<ShopifySettings> shopifySettings)
        {
            ShopifySettings = shopifySettings.Value;
        }

        protected HttpRequestMessage CreateRequest(string accessToken)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(ShopifyApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }
    }
}
