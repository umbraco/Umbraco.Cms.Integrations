
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public interface IAprimoAuthorizationService
    {
        string GetAuthorizationUrl(OAuthCodeExchange oauthCodeExchange);

        string GetAccessToken(string code);

        Task<string> GetAccessTokenAsync(string code);

        string RefreshAccessToken();

        Task<string> RefreshAccessTokenAsync();
    }
}
