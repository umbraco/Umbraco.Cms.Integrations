

using System;
using System.Threading.Tasks;

using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using System.Net.Http;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;

#if NETCOREAPP
using Microsoft.Extensions.Options;
#else
using System.Configuration;
#endif

namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public class AuthorizationService : BaseAuthorizationService, ISemrushAuthorizationService
    {
        private readonly SemrushOAuthSettings _oauthSettings;

#if NETCOREAPP
        public AuthorizationService(IOptions<SemrushOAuthSettings> options, TokenBuilder tokenBuilder, ISemrushTokenService semrushTokenService)
#else
        public AuthorizationService(TokenBuilder tokenBuilder, ISemrushTokenService semrushTokenService)
#endif
            : base(tokenBuilder, semrushTokenService)
        {
#if NETCOREAPP
            _oauthSettings = options.Value;
#else
            _oauthSettings = new SemrushOAuthSettings(ConfigurationManager.AppSettings);
#endif
        }

        public string GetAccessToken(string code) => 
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var requestData = TokenBuilder.ForAccessToken(code)
                .WithClientId(_oauthSettings.ClientId)
                .WithClientSecret(_oauthSettings.ClientSecret)
                .Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_oauthSettings.TokenEndpoint),
                Content = new FormUrlEncodedContent(requestData),
            };
            
            var response = await ClientFactory().SendAsync(requestMessage);

            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                SemrushTokenService.SaveParameters(Constants.TokenDbKey, result);

                return result;
            }

            return "Error: " + result;
        }

        public string GetAuthorizationUrl() => string.Format(SemrushAuthorizationUrl,
            _oauthSettings.Ref, _oauthSettings.ClientId, _oauthSettings.RedirectUri, _oauthSettings.Scopes);

        public string RefreshAccessToken() => 
            RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();       

        public async Task<string> RefreshAccessTokenAsync()
        {
            SemrushTokenService.TryGetParameters(Constants.TokenDbKey, out TokenDto token);

            var requestData = TokenBuilder
                .ForRefreshToken(token.RefreshToken)
                .WithClientId(_oauthSettings.ClientId)
                .WithClientSecret(_oauthSettings.ClientSecret)
                .Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_oauthSettings.TokenEndpoint),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service", "Semrush");

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                SemrushTokenService.SaveParameters(Constants.TokenDbKey, result);

                return result;
            }

            return "error";
        }
    }
}
