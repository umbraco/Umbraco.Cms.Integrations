using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Models;
using Umbraco.Cms.Integrations.Shared.Models.Dtos;
using Umbraco.Cms.Integrations.Shared.Resolvers;
using Umbraco.Cms.Integrations.Shared.Services;
using Umbraco.Cms.Integrations.Commerce.Shopify.Resources;

#if NETCOREAPP
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#else
using System.Configuration;
using Umbraco.Core.Logging;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public class ShopifyService: BaseService, IApiService<ProductsListDto>
    {
        private readonly JsonSerializerSettings _serializerSettings;

        private ShopifySettings Options;

#if NETCOREAPP
        public ShopifyService(ILogger<ShopifyService> logger, IOptions<ShopifySettings> options, ITokenService tokenService) : base(logger, tokenService)
        {
            var resolver = new JsonPropertyRenameContractResolver();
            resolver.RenameProperty(typeof(ResponseDto<ProductsListDto>), "Result", "products");

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };

            Options = options.Value;
        }
#else
        public ShopifyService(ILogger logger, ITokenService tokenService): base(logger, tokenService)
        {
            var resolver = new JsonPropertyRenameContractResolver();
            resolver.RenameProperty(typeof(ResponseDto<ProductsListDto>), "Result", "products");

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };

            Options = new ShopifySettings(ConfigurationManager.AppSettings);
        }
#endif

        public EditorSettings GetApiConfiguration()
        {
            if (string.IsNullOrEmpty(Options.Shop)
                || string.IsNullOrEmpty(Options.ApiVersion))
                return new EditorSettings();

            return
                !string.IsNullOrEmpty(Options.AccessToken)
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
                    Options.Shop, SettingsService.OAuthClientId, SettingsService.ShopifyOAuthProxyUrl);
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
            requestMessage.Headers.Add(SettingsService.ServiceAddressReplace, Options.Shop);

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

        public async Task<ResponseDto<ProductsListDto>> ValidateAccessToken()
        {
            TokenService.TryGetParameters(SettingsService.AccessTokenDbKey, out string accessToken);

            if (string.IsNullOrEmpty(accessToken))
            {
#if NETCOREAPP
                UmbCoreLogger.LogInformation(LoggingResources.AccessTokenMissing);
#else
                UmbCoreLogger.Info<ShopifyService>(message: LoggingResources.AccessTokenMissing);
#endif

                return new ResponseDto<ProductsListDto>();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(SettingsService.ProductsApiEndpoint,
                    Options.Shop,
                    Options.ApiVersion))
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            return new ResponseDto<ProductsListDto>
            {
                IsValid = response.IsSuccessStatusCode,
                IsExpired = response.StatusCode == HttpStatusCode.Unauthorized
            };
        }

        public void RevokeAccessToken()
        {
            TokenService.RemoveParameters(Constants.UmbracoCmsIntegrationsCommerceShopifyAccessToken);
        }

        public async Task<ResponseDto<ProductsListDto>> GetResults()
        {
            string accessToken;
            if (GetApiConfiguration().Type.Value == ConfigurationType.OAuth.Value)
                TokenService.TryGetParameters(SettingsService.AccessTokenDbKey, out accessToken);
            else
            {
                accessToken = Options.AccessToken;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
#if NETCOREAPP
                UmbCoreLogger.LogInformation(LoggingResources.AccessTokenMissing);
#else
                UmbCoreLogger.Info<ShopifyService>(message: LoggingResources.AccessTokenMissing);
#endif

                return new ResponseDto<ProductsListDto>();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(SettingsService.ProductsApiEndpoint,
                    Options.Shop,
                    Options.ApiVersion))
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
#if NETCOREAPP
                UmbCoreLogger.LogError(string.Format(LoggingResources.FetchProductsFailed, response.ReasonPhrase));
#else
                UmbCoreLogger.Error<ShopifyService>(string.Format(LoggingResources.FetchProductsFailed, response.ReasonPhrase));
#endif

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
