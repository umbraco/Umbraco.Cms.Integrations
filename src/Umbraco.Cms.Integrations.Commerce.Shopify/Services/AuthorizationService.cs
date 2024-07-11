#if NETCOREAPP
using Microsoft.Extensions.Options;
#else
using System.Configuration;
#endif

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Services
{
    public class AuthorizationService : BaseAuthorizationService, IShopifyAuthorizationService
    {
        private readonly ShopifySettings _settings;

        private readonly ShopifyOAuthSettings _oauthSettings;

#if NETCOREAPP
        public AuthorizationService(IOptions<ShopifySettings> options, IOptions<ShopifyOAuthSettings> oauthSettings, ITokenService tokenService)
#else
        public AuthorizationService(ITokenService tokenService)
#endif
            : base(tokenService) 
        {
#if NETCOREAPP
            _settings = options.Value;
            _oauthSettings = oauthSettings.Value;
#else
            _settings = new ShopifySettings(ConfigurationManager.AppSettings);
            _oauthSettings = new ShopifyOAuthSettings(ConfigurationManager.AppSettings);
#endif
        }

        public string GetAccessToken(string code) =>
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new Dictionary<string, string>
            {
                { "client_id", _oauthSettings.ClientId },
                { "client_secret", _oauthSettings.ClientSecret },
                { "redirect_uri", _oauthSettings.RedirectUri },
                { "code", code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_oauthSettings.TokenEndpoint),
                Content = new FormUrlEncodedContent(data)
            };

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

        public string GetAuthorizationUrl() =>
            string.Format(ShopifyAuthorizationUrl,
            _settings.Shop,
            _oauthSettings.ClientId,
            _oauthSettings.RedirectUri);
    }
}
