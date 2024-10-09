using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public class UmbracoAuthorizationService : BaseAuthorizationService, ISemrushAuthorizationService
    {
        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364;

        public const string AuthProxyTokenEndpoint = "oauth/v1/token";

        public UmbracoAuthorizationService(TokenBuilder tokenBuilder, ISemrushTokenService semrushTokenService) 
            : base(tokenBuilder, semrushTokenService)
        {
        }

        public string GetAccessToken(string code) => 
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var requestData = TokenBuilder.ForAccessToken(code).Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{OAuthProxyBaseUrl}{AuthProxyTokenEndpoint}"),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service_name", "Semrush");

            var response = await ClientFactory().SendAsync(requestMessage);

            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                SemrushTokenService.SaveParameters(Constants.TokenDbKey, result);

                return result;
            }

            return "Error: " + result;
        }

        public string GetAuthorizationUrl() =>
            string.Format(SemrushAuthorizationUrl, 
                "0053752252", 
                "umbraco", 
                "%2Foauth2%2Fumbraco%2Fsuccess", 
                "user.id,domains.info,url.info,positiontracking.info");

        public string RefreshAccessToken() => 
            RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> RefreshAccessTokenAsync()
        {
            SemrushTokenService.TryGetParameters(Constants.TokenDbKey, out TokenDto token);

            var requestData = TokenBuilder.ForRefreshToken(token.RefreshToken).Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{OAuthProxyBaseUrl}{AuthProxyTokenEndpoint}"),
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
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var statusObject = JsonSerializer.Deserialize<JsonObject>(responseContent);
                if (statusObject.ContainsKey("status") && statusObject["status"].ToString() == Constants.BadRefreshToken)
                {
                    SemrushTokenService.RemoveParameters(Constants.TokenDbKey);
                }
            }

            return "error";
        }
    }
}
