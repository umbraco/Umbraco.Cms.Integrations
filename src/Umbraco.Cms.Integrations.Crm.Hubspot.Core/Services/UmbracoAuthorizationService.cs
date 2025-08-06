using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using System.Net;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;

using Microsoft.Extensions.Options;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services
{
    public class UmbracoAuthorizationService : BaseAuthorizationService, IHubspotAuthorizationService
    {
        private readonly HubspotSettings _settings;

        public const string ClientId = "1a04f5bf-e99e-48e1-9d62-6c25bf2bdefe";

        public const string RedirectUri = OAuthProxyBaseUrl;

        public const string Service = "HubspotForms";

        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364;

        public const string OAuthProxyTokenEndpoint = "{0}oauth/v1/token";

        public const string OAuthScopes = "oauth forms crm.objects.contacts.read crm.objects.contacts.write";

        public UmbracoAuthorizationService(IOptions<HubspotSettings> options, ITokenService tokenService)
            : base(tokenService)
        {
            _settings = options.Value;
        }

        public string GetAuthorizationUrl() =>
             string.Format(HubspotAuthorizationUrl, ClientId, OAuthProxyBaseUrl, OAuthScopes);

        public string GetAccessToken(string code) => 
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", ClientId },
                { "redirect_uri", OAuthProxyBaseUrl },
                { "code", code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(OAuthProxyTokenEndpoint, OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", Service);

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

                return string.Format("{0}: {1}", ErrorPrefix, errorDto.Message);
            }

            return string.Format("{0}: An unexpected error occurred.", ErrorPrefix);
        }

        public string RefreshAccessToken() =>
            RefreshAccessTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();   

        public async Task<string> RefreshAccessTokenAsync()
        {
            TokenService.TryGetParameters(Constants.RefreshTokenDbKey, out string refreshToken);

            var data = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "client_id", ClientId },
                { "refresh_token", refreshToken }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(OAuthProxyTokenEndpoint, OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", Service);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(result);

                TokenService.SaveParameters(Constants.AccessTokenDbKey, tokenDto.AccessToken);
                TokenService.SaveParameters(Constants.RefreshTokenDbKey, tokenDto.RefreshToken);

                return result;
            }

            return ErrorPrefix;
        }
    }
}
