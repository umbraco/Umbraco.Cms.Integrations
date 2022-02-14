using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Configuration;
using Umbraco.Cms.Integrations.Shared.Controllers;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Cms.Integrations.Shared.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Resolvers;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCommerceShopify")]
    public class ProductsController : BaseAuthorizedApiController
    {
        public const string ProductsApiEndpoint = "https://{0}.myshopify.com/admin/api/{1}/products.json";

        private readonly JsonSerializerSettings _serializerSettings;

        public ProductsController(ILogger logger, IAppSettings appSettings) : base(logger, appSettings)
        {
            var resolver = new JsonPropertyRenameContractResolver();
            resolver.RenameProperty(typeof(ResponseDto<ProductsListDto>), "Result", "products");

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.ContractResolver = resolver;
        }

        public async Task<ResponseDto<ProductsListDto>> GetList()
        {
            var accessToken = AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyAccessToken];

            if (string.IsNullOrEmpty(accessToken))
            {
                ApiLogger.Info<ProductsController>(message: "Cannot access Shopify - Access Token is missing.");

                return new ResponseDto<ProductsListDto>();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(ProductsApiEndpoint,
                    AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop],
                    AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyApiVersion]))
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ApiLogger.Error<ProductsController>($"Failed to fetch products from Shopify store using access token: {response.ReasonPhrase}");

                return new ResponseDto<ProductsListDto> { Message = response.ReasonPhrase };
            }

            if (response.IsSuccessStatusCode)
            {
               

                var result = await response.Content.ReadAsStringAsync();
                return new ResponseDto<ProductsListDto>
                {
                    IsValid = true,
                    Result = JsonConvert.DeserializeObject<ProductsListDto>(result, _serializerSettings)
                };
            }

            return new ResponseDto<ProductsListDto>();
        }
    }
}
