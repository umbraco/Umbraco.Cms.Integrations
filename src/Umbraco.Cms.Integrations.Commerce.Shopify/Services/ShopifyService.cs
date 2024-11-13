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
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web;
using Umbraco.Cms.Integrations.Commerce.Shopify.Helpers;





#if NETCOREAPP
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopifyLogLevel = Umbraco.Cms.Core.Logging.LogLevel;
#else
using System.Configuration;
using Umbraco.Core.Logging;
using ShopifyLogLevel = Umbraco.Core.Logging.LogLevel;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public class ShopifyService : IShopifyService
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

        public async Task<EditorSettings> GetApiConfiguration()
        {
            if (string.IsNullOrEmpty(_settings.Shop)
                || string.IsNullOrEmpty(_settings.ApiVersion))
                return new EditorSettings();

            // validate API configuration
            if (!string.IsNullOrEmpty(_settings.AccessToken))
                return new EditorSettings { IsValid = true, Type = ConfigurationType.Api };

            // validate OAuth configuration if AuthorizationService is used.
            // if authorization is managed through UmbracoAuthorizationService, the properties client ID and proxy URL are set correctly.
            var accessTokenValidationResponse = await ValidateAccessToken();
            if (_settings.UseUmbracoAuthorization)
            {
                var editorSettings = new EditorSettings { IsValid = true, Type = ConfigurationType.OAuth };

                editorSettings.IsConnected = accessTokenValidationResponse.IsValid;

                return editorSettings;
            }
            else
            {
                return !string.IsNullOrEmpty(_oauthSettings.ClientId)
                        && !string.IsNullOrEmpty(_oauthSettings.ClientSecret)
                        && !string.IsNullOrEmpty(_oauthSettings.RedirectUri)
                        ? new EditorSettings { 
                            IsValid = true,
                            IsConnected = accessTokenValidationResponse.IsValid, 
                            Type = ConfigurationType.OAuth 
                        }
                        : new EditorSettings();
            }
        }

        public async Task<ResponseDto<ProductsListDto>> ValidateAccessToken()
        {
            _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);

            if (string.IsNullOrEmpty(accessToken))
            {
                Log(ShopifyLogLevel.Information, LoggingResources.AccessTokenMissing);

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
            _tokenService.RemoveParameters(Constants.AccessTokenDbKey);
        }

        public async Task<ResponseDto<ProductsListDto>> GetResults(string pageInfo)
        {
            string accessToken;

            var apiConfiguration = await GetApiConfiguration();
            if (apiConfiguration.Type.Value == ConfigurationType.OAuth.Value)
                _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out accessToken);
            else
            {
                accessToken = _settings.AccessToken;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                Log(ShopifyLogLevel.Information, LoggingResources.AccessTokenMissing);

                return new ResponseDto<ProductsListDto>();
            }

            var requestUrl = string.IsNullOrEmpty(pageInfo)
                ? string.Format(Constants.ProductsApiEndpoint,
                    _settings.Shop,
                    _settings.ApiVersion) + "?limit=" + Constants.DEFAULT_PAGE_SIZE
                : string.Format(Constants.ProductsApiEndpoint,
                    _settings.Shop,
                    _settings.ApiVersion) + "?page_info=" + pageInfo + "&limit=" + Constants.DEFAULT_PAGE_SIZE;
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUrl)
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Log(ShopifyLogLevel.Error, string.Format(LoggingResources.FetchProductsFailed, response.ReasonPhrase));

                return new ResponseDto<ProductsListDto> { Message = response.ReasonPhrase };
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseDto = new ResponseDto<ProductsListDto>
                {
                    IsValid = true,
                    Result = JsonConvert.DeserializeObject<ProductsListDto>(result, _serializerSettings)
                };

                var pageInfoDetails = response.GetPageInfo();
                if (pageInfoDetails != null)
                {
                    responseDto.PreviousPageInfo = pageInfoDetails.Item1;
                    responseDto.NextPageInfo = pageInfoDetails.Item2;
                }

                return responseDto;
            }

            return new ResponseDto<ProductsListDto>();
        }

        public async Task<ResponseDto<ProductsListDto>> GetProductsByIds(long[] ids)
        {
            string accessToken;
            var apiConfiguration = await GetApiConfiguration();
            if (apiConfiguration.Type.Value == ConfigurationType.OAuth.Value)
                _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out accessToken);
            else
            {
                accessToken = _settings.AccessToken;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                Log(ShopifyLogLevel.Information, LoggingResources.AccessTokenMissing);

                return new ResponseDto<ProductsListDto>();
            }

            var requestUrl = string.Format(Constants.ProductsApiEndpoint,
                    _settings.Shop,
                    _settings.ApiVersion) + "?ids=" + string.Join(",", ids);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUrl)
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Log(ShopifyLogLevel.Error, string.Format(LoggingResources.FetchProductsFailed, response.ReasonPhrase));

                return new ResponseDto<ProductsListDto> { Message = response.ReasonPhrase };
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseDto = new ResponseDto<ProductsListDto>
                {
                    IsValid = true,
                    Result = JsonConvert.DeserializeObject<ProductsListDto>(result, _serializerSettings)
                };

                return responseDto;
            }

            return new ResponseDto<ProductsListDto>();
        }

        public async Task<int> GetCount()
        {
            string accessToken;
            var apiConfiguration = await GetApiConfiguration();
            if (apiConfiguration.Type.Value == ConfigurationType.OAuth.Value)
                _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out accessToken);
            else
            {
                accessToken = _settings.AccessToken;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                Log(ShopifyLogLevel.Information, LoggingResources.AccessTokenMissing);

                return 0;
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(Constants.ProductsCountApiEndpoint,
                    _settings.Shop,
                    _settings.ApiVersion))
            };
            requestMessage.Headers.Add("X-Shopify-Access-Token", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Log(ShopifyLogLevel.Information, responseContent);

                return 0;
            }

            var result = await response.Content.ReadAsStringAsync();
            return ((JObject)JsonConvert.DeserializeObject(result)).Value<int>("count");
        }

        private void Log(ShopifyLogLevel logLevel, string message)
        {
            if (logLevel == ShopifyLogLevel.Error)
            {
#if NETCOREAPP
                _umbCoreLogger.LogError(message);
#else
                _umbCoreLogger.Error<ShopifyService>(message);
#endif
            }
            else if (logLevel == ShopifyLogLevel.Information)
            {
#if NETCOREAPP
                _umbCoreLogger.LogInformation(message);
#else
                _umbCoreLogger.Info<ShopifyService>(message: message);
#endif
            }
        }
    }
}
