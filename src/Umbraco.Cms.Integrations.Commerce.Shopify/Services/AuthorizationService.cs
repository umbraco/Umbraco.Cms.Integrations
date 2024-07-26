using Microsoft.Extensions.Options;
using System.Net;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public class AuthorizationService : BaseAuthorizationService, IShopifyAuthorizationService
    {
        private readonly ShopifySettings _settings;

        private readonly ShopifyOAuthSettings _oauthSettings;

        public AuthorizationService(IOptions<ShopifySettings> options, IOptions<ShopifyOAuthSettings> oauthSettings, ITokenService tokenService)
            : base(tokenService) 
        {
            _settings = options.Value;
            _oauthSettings = oauthSettings.Value;
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

                var tokenDto = JsonSerializer.Deserialize<TokenDto>(result);

                TokenService.SaveParameters(Constants.AccessTokenDbKey, tokenDto.AccessToken);

                return result;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResult = await response.Content.ReadAsStringAsync();
                var errorDto = JsonSerializer.Deserialize<ErrorDto>(errorResult);

                return "Error: " + errorDto.Message;
            }

            return "Error: An unexpected error occurred.";
        }

        public string GetAuthorizationUrl() =>
            string.Format(ShopifyAuthorizationUrl,
            _settings.Shop,
            _oauthSettings.ClientId,
            _oauthSettings.RedirectUri);

        public string RefreshAccessToken() => RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> RefreshAccessTokenAsync()
        {
            TokenService.TryGetParameters(Constants.RefreshTokenDbKey, out string refreshToken);

            var data = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"client_id", _oauthSettings.ClientId },
                {"client_secret", _oauthSettings.ClientSecret },
                { "refresh_token", refreshToken }
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

                var tokenDto = JsonSerializer.Deserialize<TokenDto>(result);

                TokenService.SaveParameters(Constants.AccessTokenDbKey, tokenDto.AccessToken);
                TokenService.SaveParameters(Constants.RefreshTokenDbKey, tokenDto.RefreshToken);

                return result;
            }

            return "error";
        }
    }
}
