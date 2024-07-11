using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos;
using Microsoft.Extensions.Options;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Services
{
    public class UmbracoAuthorizationService : BaseAuthorizationService, IShopifyAuthorizationService
    {
        private readonly ShopifySettings _settings;

        public const string Service = "Shopify";

        public const string ClientId = "23c1bc3c70de807d84b79a29b12b49f5";

        public const string ServiceAddressReplace = "service_address_shop-replace";

        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/";  // for local testing: "https://localhost:44364/";

        public const string OAuthProxyRedirectUrl = OAuthProxyBaseUrl + "oauth/shopify";

        public const string OAuthProxyTokenUrl = OAuthProxyBaseUrl + "oauth/v1/token";
        public UmbracoAuthorizationService(IOptions<ShopifySettings> options, ITokenService tokenService)
            : base(tokenService) 
        {
            _settings = options.Value;
        }

        public string GetAccessToken(string code) => 
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "redirect_uri", OAuthProxyRedirectUrl },
                { "code", code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthProxyTokenUrl),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", Service);
            requestMessage.Headers.Add(ServiceAddressReplace, _settings.Shop);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                TokenService.SaveParameters(Constants.AccessTokenDbKey, tokenDto.AccessToken);

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

        public string GetAuthorizationUrl() => string.Format(ShopifyAuthorizationUrl,
            _settings.Shop,
            ClientId,
            OAuthProxyRedirectUrl);
    }
}
