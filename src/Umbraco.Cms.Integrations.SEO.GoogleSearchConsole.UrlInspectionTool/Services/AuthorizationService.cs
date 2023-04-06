

using System.Threading.Tasks;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;

#if NETCOREAPP
using Microsoft.Extensions.Options;
#else
using System.Configuration;
#endif

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public class AuthorizationService : BaseAuthorizationService, IGoogleAuthorizationService
    {
        private readonly GoogleSearchConsoleOAuthSettings _oauthSettings;

#if NETCOREAPP
        public AuthorizationService(IOptions<GoogleSearchConsoleOAuthSettings> options, ITokenService tokenService) : base(tokenService)
        {
            _oauthSettings = options.Value;
        }
#else
        public AuthorizationService(ITokenService tokenService) : base(tokenService)
        {
            _oauthSettings = new GoogleSearchConsoleOAuthSettings(ConfigurationManager.AppSettings);
        }
#endif

        public string GetAccessToken(string code) =>
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var requestData = new Dictionary<string, string>
            {
                {"code", code },
                {"client_id", _oauthSettings.ClientId },
                {"client_secret", _oauthSettings.ClientSecret },
                {"redirect_uri", _oauthSettings.RedirectUri },
                {"grant_type", "authorization_code" }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_oauthSettings.TokenEndpoint),
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

        public string GetAuthorizationUrl() =>
            string.Format(SearchConsoleAuthorizationUrl, _oauthSettings.RedirectUri, _oauthSettings.ClientId, string.Join(" ", _oauthSettings.Scopes));

        public string RefreshAccessToken() =>
            RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> RefreshAccessTokenAsync()
        {
            TokenService.TryGetParameters(Constants.RefreshTokenDbKey, out string refreshToken);

            var requestData = new Dictionary<string, string>
            {
                {"client_id", _oauthSettings.ClientId },
                {"client_secret", _oauthSettings.ClientSecret },
                {"refresh_token", refreshToken },
                {"grant_type", "refresh_token" }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_oauthSettings.TokenEndpoint),
                Content = new FormUrlEncodedContent(requestData),
            };

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
