using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Configuration;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;


namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services
{
    public class AuthorizationService : BaseAuthorizationService, IHubspotAuthorizationService
    {
        private readonly HubspotOAuthSettings _oauthSettings;

        public AuthorizationService(ITokenService tokenService)
            : base(tokenService)
        {
            _oauthSettings = new HubspotOAuthSettings(ConfigurationManager.AppSettings);
        }

        public string GetAuthorizationUrl() => string.Format(HubspotAuthorizationUrl,
            _oauthSettings.ClientId,
            _oauthSettings.RedirectUri,
            _oauthSettings.Scopes);

        public string GetAccessToken(string code) =>
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
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
                TokenService.SaveParameters(Constants.RefreshTokenDbKey, tokenDto.RefreshToken);

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

        public string RefreshAccessToken() =>
            RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                TokenService.SaveParameters(Constants.AccessTokenDbKey, tokenDto.AccessToken);
                TokenService.SaveParameters(Constants.RefreshTokenDbKey, tokenDto.RefreshToken);

                return result;
            }

            return "error";
        }
    }
}
