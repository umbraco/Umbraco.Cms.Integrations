﻿using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services
{
    public interface IHubspotAuthorizationService
    {
        string GetAuthorizationUrl();

        string GetAccessToken(string code);

        Task<string> GetAccessTokenAsync(string code);

        string RefreshAccessToken();

        Task<string> RefreshAccessTokenAsync();
    }
}
