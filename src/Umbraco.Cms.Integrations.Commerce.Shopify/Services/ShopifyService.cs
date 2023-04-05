using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Resolvers;
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
    public class ShopifyService: IShopifyService
    {
        private readonly JsonSerializerSettings _serializerSettings;

        private readonly ShopifySettings _settings;

        private readonly ShopifyOAuthSettings _oauthSettings;

#if NETCOREAPP
        private readonly ILogger<ShopifyService> _umbCoreLogger;
#else
        private readonly ILogger _umbCoreLogger;
#endif

        private readonly ITokenService _tokenService;

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private static readonly HttpClient Client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => Client;

#if NETCOREAPP
        public ShopifyService(ILogger<ShopifyService> logger, 
            IOptions<ShopifySettings> options, IOptions<ShopifyOAuthSettings> oauthOptions, 
            ITokenService tokenService)
        {
            var resolver = new JsonPropertyRenameContractResolver();
            resolver.RenameProperty(typeof(ResponseDto<ProductsListDto>), "Result", "products");

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };

            _settings = options.Value;
            _oauthSettings = oauthOptions.Value;

            _umbCoreLogger = logger;

            _tokenService = tokenService;
        }
#else
        public ShopifyService(ILogger logger, ITokenService tokenService)
        {
            var resolver = new JsonPropertyRenameContractResolver();
            resolver.RenameProperty(typeof(ResponseDto<ProductsListDto>), "Result", "products");

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };

            _settings = new ShopifySettings(ConfigurationManager.AppSettings);
            _oauthSettings = new ShopifyOAuthSettings(ConfigurationManager.AppSettings);

            _umbCoreLogger = logger;

            _tokenService = tokenService;
        }
#endif

        public EditorSettings GetApiConfiguration()
        {
            if (string.IsNullOrEmpty(_settings.Shop)
                || string.IsNullOrEmpty(_settings.ApiVersion))
                return new EditorSettings();

            // validate API configuration
            if (!string.IsNullOrEmpty(_settings.AccessToken))
                return new EditorSettings { IsValid = true, Type = ConfigurationType.Api };

            // validate OAuth configuration if AuthorizationService is used.
            // if authorization is managed through UmbracoAuthorizationService, the properties client ID and proxy URL are set correctly.
            if (_settings.UseUmbracoAuthorization)
                return new EditorSettings { IsValid = true, Type = ConfigurationType.OAuth };
            else
                return !string.IsNullOrEmpty(_oauthSettings.ClientId)
                        && !string.IsNullOrEmpty(_oauthSettings.ClientSecret)
                        && !string.IsNullOrEmpty(_oauthSettings.RedirectUri)
                        ? new EditorSettings { IsValid = true, Type = ConfigurationType.OAuth }
                        : new EditorSettings();
        }

        public async Task<ResponseDto<ProductsListDto>> ValidateAccessToken()
        {
            _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);

            if (string.IsNullOrEmpty(accessToken))
            {
#if NETCOREAPP
                _umbCoreLogger.LogInformation(LoggingResources.AccessTokenMissing);
#else
                _umbCoreLogger.Info<ShopifyService>(message: LoggingResources.AccessTokenMissing);
#endif

                return new ResponseDto<ProductsListDto>();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(Constants.ProductsApiEndpoint,
                    _settings.Shop,
                    _settings.ApiVersion))
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
            _tokenService.RemoveParameters(Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyAccessToken);
        }

        public async Task<ResponseDto<ProductsListDto>> GetResults()
        {
            string accessToken;
            if (GetApiConfiguration().Type.Value == ConfigurationType.OAuth.Value)
                _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out accessToken);
            else
            {
                accessToken = _settings.AccessToken;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
#if NETCOREAPP
                _umbCoreLogger.LogInformation(LoggingResources.AccessTokenMissing);
#else
                _umbCoreLogger.Info<ShopifyService>(message: LoggingResources.AccessTokenMissing);
#endif

                return new ResponseDto<ProductsListDto>();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(Constants.ProductsApiEndpoint,
                    _settings.Shop,
                    _settings.ApiVersion))
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
#if NETCOREAPP
                _umbCoreLogger.LogError(string.Format(LoggingResources.FetchProductsFailed, response.ReasonPhrase));
#else
                _umbCoreLogger.Error<ShopifyService>(string.Format(LoggingResources.FetchProductsFailed, response.ReasonPhrase));
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
