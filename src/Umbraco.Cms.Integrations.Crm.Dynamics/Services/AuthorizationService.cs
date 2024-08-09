using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Microsoft.Extensions.Options;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class AuthorizationService : BaseAuthorizationService, IDynamicsAuthorizationService
    {
        private readonly DynamicsOAuthSettings _oauthSettings;

        public AuthorizationService(IOptions<DynamicsOAuthSettings> oauthOptions,
            DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService)
                : base(dynamicsService, dynamicsConfigurationService)

        {
            _oauthSettings = oauthOptions.Value;
        }

        public string GetAuthorizationUrl() => string.Format(DynamicsAuthorizationUrl,
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

                var identity = await DynamicsService.GetIdentity(tokenDto.AccessToken);

                if (identity.IsAuthorized)
                    DynamicsConfigurationService.AddorUpdateOAuthConfiguration(tokenDto.AccessToken, identity.UserId, identity.FullName);
                else
                    return "Error: " + identity.Error.Message;

                return result;
            }

            var errorResult = await response.Content.ReadAsStringAsync();
            var errorDto = JsonConvert.DeserializeObject<ErrorDto>(errorResult);

            return "Error: " + errorDto.ErrorDescription;
        }
    }
}
