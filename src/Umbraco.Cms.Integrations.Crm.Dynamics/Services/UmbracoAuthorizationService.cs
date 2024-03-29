﻿using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

#if NETCOREAPP
using Microsoft.Extensions.Options;
#else
using System.Configuration;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class UmbracoAuthorizationService : BaseAuthorizationService, IDynamicsAuthorizationService
    {
        private readonly DynamicsSettings _settings;

        public const string ClientId = "813c5a65-cfd6-48d6-8928-dffe02aaf61a";

        public const string Service = "Dynamics";

        public const string OAuthProxyBaseUrl = "https://hubspot-forms-auth.umbraco.com/"; // for local testing: https://localhost:44364;

        public const string OAuthProxyRedirectUrl = OAuthProxyBaseUrl + "oauth/dynamics/";

        public const string OAuthProxyTokenUrl = OAuthProxyBaseUrl + "oauth/v1/token";

        protected const string OAuthScopes = "{0}.default";

#if NETCOREAPP
        public UmbracoAuthorizationService(IOptions<DynamicsSettings> options, 
            DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService) 
            : base(dynamicsService, dynamicsConfigurationService)
        {
            _settings = options.Value;
        }
#else
        public UmbracoAuthorizationService(DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService)
            : base(dynamicsService, dynamicsConfigurationService)
        {
            _settings = new DynamicsSettings(ConfigurationManager.AppSettings);
        }
#endif

        public string GetAuthorizationUrl()
        {
            var scopes = string.Format(OAuthScopes, _settings.HostUrl);
            return string.Format(DynamicsAuthorizationUrl, ClientId, OAuthProxyRedirectUrl, scopes);
        }

        public string GetAccessToken(string code) => 
            GetAccessTokenAsync(code).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", ClientId },
                { "redirect_uri", OAuthProxyRedirectUrl },
                { "code", code }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthProxyTokenUrl),
                Content = new FormUrlEncodedContent(data)
            };
            requestMessage.Headers.Add("service_name", Service);

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
