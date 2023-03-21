using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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

        public UmbracoAuthorizationService(ITokenService tokenService) : base(tokenService)
        {
        }

        public string GetAuthorizationUrl() => 
            string.Format(SearchConsoleAuthorizationUrl, OAuthProxyRedirectUrl, ClientId, string.Join(" ", Scopes));

        public string GetAccessToken(string code) =>
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var requestData = new Dictionary<string, string>
            {
                {"code", code },
                {"client_id", ClientId },
                {"redirect_uri", OAuthProxyRedirectUrl },
                {"grant_type", "authorization_code" }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthProxyTokenUrl),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service_name", "Google");

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                TokenService.SaveParameters(Constants.TokenDbKey, tokenDto.AccessToken);
                TokenService.SaveParameters(Constants.RefreshTokenDbKey, tokenDto.RefreshToken);

                return result;
            }

            return "error";
        }

        public string RefreshAccessToken() => RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> RefreshAccessTokenAsync()
        {
            TokenService.TryGetParameters(Constants.RefreshTokenDbKey, out string refreshToken);

            var requestData = new Dictionary<string, string>
            {
                {"client_id", ClientId },
                {"refresh_token", refreshToken },
                {"grant_type", "refresh_token" }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthProxyTokenUrl),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service_name", "Google");

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                TokenService.SaveParameters(Constants.TokenDbKey, tokenDto.AccessToken);

                return result;
            }

            return "error";
        }
    }
}
