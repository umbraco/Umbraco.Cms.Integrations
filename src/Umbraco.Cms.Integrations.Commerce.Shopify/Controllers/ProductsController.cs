using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Configuration;
using Umbraco.Cms.Integrations.Shared.Controllers;
using Umbraco.Cms.Integrations.Shared.Models;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Cms.Integrations.Shared.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Resolvers;
using Umbraco.Cms.Integrations.Shared.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCommerceShopify")]
    public class ProductsController : BaseAuthorizedApiController
    {
        private const string ProductsApiEndpoint = "https://{0}.myshopify.com/admin/api/{1}/products.json";

        private const string OAuthClientId = "23c1bc3c70de807d84b79a29b12b49f5";

        private const string ShopifyServiceName = "Shopify";
        private const string ShopifyServiceAddressReplace = "service_address_shop-replace";

        private string ShopifyOAuthProxyUrl = $"{OAuthProxyBaseUrl}oauth/shopify";
        private const string ShopifyAuthorizationUrl =
            "https://{0}.myshopify.com/admin/oauth/authorize?client_id={1}&redirect_uri={2}&scope=read_products&grant_options[]=value";

        private const string AccessTokenDbKey = "Umbraco.Cms.Integrations.Shopify.AccessTokenDbKey";

        private readonly JsonSerializerSettings _serializerSettings;

        public ProductsController(ILogger logger, IAppSettings appSettings, ITokenService tokenService) : base(logger, appSettings, tokenService)
        {
            var resolver = new JsonPropertyRenameContractResolver();
            resolver.RenameProperty(typeof(ResponseDto<ProductsListDto>), "Result", "products");

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.ContractResolver = resolver;
        }

        [HttpGet]
        public EditorSettings CheckConfiguration()
        {
            if (string.IsNullOrEmpty(AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop])
                || string.IsNullOrEmpty(AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyApiVersion]))
                return new EditorSettings();

            return
                !string.IsNullOrEmpty(AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyAccessToken])
                    ? new EditorSettings { IsValid = true, Type = ConfigurationType.Api }
                    : !string.IsNullOrEmpty(OAuthClientId)
                      && !string.IsNullOrEmpty(OAuthProxyBaseUrl)
                      && !string.IsNullOrEmpty(OAuthProxyEndpoint)
                        ? new EditorSettings { IsValid = true, Type = ConfigurationType.OAuth }
                        : new EditorSettings();
        }

        [HttpGet]
        public string GetAuthorizationUrl()
        {
            return 
                string.Format(ShopifyAuthorizationUrl, 
                    AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop], OAuthClientId, ShopifyOAuthProxyUrl);
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto)
        {
            var data = new Dictionary<string, string>
            {
                { "client_id", OAuthClientId },
                { "redirect_uri", string.Format(ShopifyOAuthProxyUrl, OAuthProxyBaseUrl) },
                { "code", authRequestDto.Code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(OAuthProxyEndpoint, OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", ShopifyServiceName);
            requestMessage.Headers.Add(ShopifyServiceAddressReplace,
                AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop]);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                TokenService.SaveParameters(AccessTokenDbKey, tokenDto.AccessToken);

                return result;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResult = await response.Content.ReadAsStringAsync();
                var errorDto = JsonConvert.DeserializeObject<ErrorDto>(errorResult);

                return "Error: " + errorDto.Message;
            }

            return "Error: An unexpected error occurred.";
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

        public async Task<ResponseDto<ProductsListDto>> GetListOAuth()
        {
            TokenService.TryGetParameters(AccessTokenDbKey, out string accessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                ApiLogger.Info<ProductsController>("Cannot access Shopify - Access Token is missing.");

                return new ResponseDto<ProductsListDto>();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("")
            };

            return new ResponseDto<ProductsListDto>();
        }
    }
}
