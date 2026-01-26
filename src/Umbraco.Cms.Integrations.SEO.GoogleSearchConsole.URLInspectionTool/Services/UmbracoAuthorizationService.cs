using System.Text.Json;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public class UmbracoAuthorizationService : BaseAuthorizationService, IGoogleAuthorizationService
    {
        protected const string OAuthProxyTokenUrl = "https://hubspot-forms-auth.umbraco.com/oauth/v1/token";

        protected const string OAuthProxyRedirectUrl = "https://hubspot-forms-auth.umbraco.com/oauth/google";

        protected string[] Scopes = new[]
        {
            "https://www.googleapis.com/auth/webmasters",
            "https://www.googleapis.com/auth/webmasters.readonly"
        };

        protected const string ClientId = "849175818654-0jtc4c8baoo58d3ruhbkghao425ejrvf.apps.googleusercontent.com";

        public UmbracoAuthorizationService(ITokenService tokenService, IHttpClientFactory httpClientFactory) : base(tokenService, httpClientFactory)
        {
        }

        public string GetAuthorizationUrl() => 
            string.Format(SearchConsoleAuthorizationUrl, OAuthProxyRedirectUrl, ClientId, string.Join(" ", Scopes));

        public string GetAccessToken(string code)
        {
            var result = GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

            if (result.Success && result.TokenDto is not null)
            {
                return result.TokenDto.AccessToken;
            }

            return string.Empty;
        }

        public async Task<GoogleSearchConsoleResult> GetAccessTokenAsync(string code)
        {
            var client = HttpClientFactory.CreateClient();

            var requestData = new Dictionary<string, string>
            {
                {"code", code },
                {"client_id", ClientId },
                {"redirect_uri", OAuthProxyRedirectUrl },
                {"grant_type", "authorization_code" }
            };

            using var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthProxyTokenUrl),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service_name", "Google");

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var tokenDto = JsonSerializer.Deserialize<TokenDto>(result);
                if (tokenDto is null)
                {
                    return new(false, "Failed to deserialize token response.");
                }

                TokenService.SaveParameters(Constants.TokenDbKey, tokenDto.AccessToken);
                TokenService.SaveParameters(Constants.RefreshTokenDbKey, tokenDto.RefreshToken);

                return new(true, TokenDto: tokenDto);
            }

            return new(false, result);
        }

        public string RefreshAccessToken()
        {
            var result = RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (result.Success && result.TokenDto is not null)
            {
                return result.TokenDto.AccessToken;
            }

            return string.Empty;
        }

        public async Task<GoogleSearchConsoleResult> RefreshAccessTokenAsync()
        {
            var client = HttpClientFactory.CreateClient();

            TokenService.TryGetParameters(Constants.RefreshTokenDbKey, out string refreshToken);

            var requestData = new Dictionary<string, string>
            {
                {"client_id", ClientId },
                {"refresh_token", refreshToken },
                {"grant_type", "refresh_token" }
            };

            using var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthProxyTokenUrl),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service_name", "Google");

            var response = await client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var tokenDto = JsonSerializer.Deserialize<TokenDto>(result);
                if (tokenDto is null)
                {
                    return new(false, "Failed to deserialize token response.");
                }

                TokenService.SaveParameters(Constants.TokenDbKey, tokenDto.AccessToken);

                return new(true, TokenDto: tokenDto);
            }

            return new(false, result);
        }
    }
}
