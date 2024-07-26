﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Dynamic;
using System.Net;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Helpers;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Resources;
using ShopifyLogLevel = Umbraco.Cms.Core.Logging.LogLevel;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public class ShopifyService : IShopifyService
    {
        private readonly ShopifySettings _settings;

        private readonly ShopifyOAuthSettings _oauthSettings;

        private readonly ILogger<ShopifyService> _umbCoreLogger;

        private readonly ITokenService _tokenService;

        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private static readonly HttpClient Client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => Client;

        public ShopifyService(ILogger<ShopifyService> logger,
            IOptions<ShopifySettings> options, IOptions<ShopifyOAuthSettings> oauthOptions,
            ITokenService tokenService)
        {
            _settings = options.Value;
            _oauthSettings = oauthOptions.Value;

            _umbCoreLogger = logger;

            _tokenService = tokenService;
        }

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
            _tokenService.RemoveParameters(Constants.Configuration.UmbracoCmsIntegrationsCommerceShopifyAccessToken);
        }

        public async Task<ResponseDto<ProductsListDto>> GetResults(string pageInfo)
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
                    Result = JsonSerializer.Deserialize<ProductsListDto>(result)
                };

                var pageInfoDetails = response.GetPageInfo();
                responseDto.PreviousPageInfo = pageInfoDetails.Item1;
                responseDto.NextPageInfo = pageInfoDetails.Item2;

                return responseDto;
            }

            return new ResponseDto<ProductsListDto>();
        }

        public async Task<ResponseDto<ProductsListDto>> GetProductsByIds(long[] ids)
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
                    Result = JsonSerializer.Deserialize<ProductsListDto>(result)
                };

                return responseDto;
            }

            return new ResponseDto<ProductsListDto>();
        }

        public async Task<int> GetCount()
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
            return JsonSerializer.Deserialize<ProductCountDto>(result).Count; 
        }

        private void Log(ShopifyLogLevel logLevel, string message)
        {
            if (logLevel == ShopifyLogLevel.Error)
            {
                _umbCoreLogger.LogError(message);
            }
            else if (logLevel == ShopifyLogLevel.Information)
            {
                _umbCoreLogger.LogInformation(message);
            }
        }
    }
}
