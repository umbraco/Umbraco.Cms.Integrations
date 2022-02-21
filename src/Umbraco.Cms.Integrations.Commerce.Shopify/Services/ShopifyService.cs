using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Configuration;
using Umbraco.Cms.Integrations.Shared.Models;
using Umbraco.Cms.Integrations.Shared.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Resolvers;
using Umbraco.Cms.Integrations.Shared.Services;
using Umbraco.Core.Logging;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public class ShopifyService: BaseService, IApiService<ProductsListDto>
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public ShopifyService(ILogger logger, IAppSettings appSettings, ITokenService tokenService): base(logger, appSettings, tokenService)
        {
            var resolver = new JsonPropertyRenameContractResolver();
            resolver.RenameProperty(typeof(ResponseDto<ProductsListDto>), "Result", "products");

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };
        }

        public EditorSettings GetApiConfiguration()
        {
            if (string.IsNullOrEmpty(AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop])
                || string.IsNullOrEmpty(AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyApiVersion]))
                return new EditorSettings();

            return
                !string.IsNullOrEmpty(AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyAccessToken])
                    ? new EditorSettings { IsValid = true, Type = ConfigurationType.Api }
                    : !string.IsNullOrEmpty(SettingsService.OAuthClientId)
                      && !string.IsNullOrEmpty(OAuthProxyBaseUrl)
                      && !string.IsNullOrEmpty(OAuthProxyEndpoint)
                        ? new EditorSettings { IsValid = true, Type = ConfigurationType.OAuth }
                        : new EditorSettings();
        }

        public string GetAuthorizationUrl()
        {
            return
                string.Format(SettingsService.AuthorizationUrl,
                    AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop], SettingsService.OAuthClientId, SettingsService.ShopifyOAuthProxyUrl);
        }

        public async Task<string> GetAccessToken(OAuthRequestDto request)
        {
            var data = new Dictionary<string, string>
            {
                { "client_id", SettingsService.OAuthClientId },
                { "redirect_uri", string.Format(SettingsService.ShopifyOAuthProxyUrl, OAuthProxyBaseUrl) },
                { "code", request.Code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(OAuthProxyEndpoint, OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", SettingsService.ServiceName);
            requestMessage.Headers.Add(SettingsService.ServiceAddressReplace,
                AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop]);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                TokenService.SaveParameters(SettingsService.AccessTokenDbKey, tokenDto.AccessToken);

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
     
        public async Task<ResponseDto<ProductsListDto>> GetResults()
        {
            string accessToken;
            if (GetApiConfiguration().Type == ConfigurationType.OAuth)
                TokenService.TryGetParameters(SettingsService.AccessTokenDbKey, out accessToken);
            else
            {
                accessToken = AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyAccessToken];
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                UmbCoreLogger.Info<ShopifyService>(message: "Cannot access Shopify - Access Token is missing.");

                return new ResponseDto<ProductsListDto>();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(SettingsService.ProductsApiEndpoint,
                    AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyShop],
                    AppSettings[Constants.UmbracoCmsIntegrationsCommerceShopifyApiVersion]))
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                UmbCoreLogger.Error<ShopifyService>($"Failed to fetch products from Shopify store using access token: {response.ReasonPhrase}");

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
