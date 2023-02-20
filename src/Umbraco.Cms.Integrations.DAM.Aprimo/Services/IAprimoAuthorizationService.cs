
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public interface IAprimoAuthorizationService
    {
        string GetAuthorizationUrl(OAuthCodeExchange oauthCodeExchange);

        Task<string> GetAccessToken(string code);

        Task<string> RefreshAccessToken();
    }
}
