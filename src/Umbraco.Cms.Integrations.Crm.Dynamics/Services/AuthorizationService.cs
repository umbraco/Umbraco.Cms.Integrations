using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class AuthorizationService : BaseAuthorizationService, IDynamicsAuthorizationService
    {
        private readonly DynamicsOAuthSettings _oauthSettings;

        public AuthorizationService(
            IOptions<DynamicsOAuthSettings> oauthOptions,
            IDynamicsService dynamicsService, 
            IDynamicsConfigurationStorage dynamicsConfigurationStorage)
            : base(dynamicsService, dynamicsConfigurationStorage)

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

                var tokenDto = JsonSerializer.Deserialize<TokenDto>(result);

                var identity = await DynamicsService.GetIdentity(tokenDto.AccessToken);

                if (identity.IsAuthorized)
                    DynamicsConfigurationStorage.AddOrUpdateOAuthConfiguration(tokenDto.AccessToken, identity.UserId, identity.FullName);
                else
                    return "Error: " + identity.Error.Message;

                return result;
            }

            var errorResult = await response.Content.ReadAsStringAsync();
            var errorDto = JsonSerializer.Deserialize<ErrorDto>(errorResult);

            return "Error: " + errorDto.ErrorDescription;
        }
    }
}
