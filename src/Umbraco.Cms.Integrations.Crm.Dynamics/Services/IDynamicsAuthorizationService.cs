using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public interface IDynamicsAuthorizationService
    {
        string GetAuthorizationUrl();

        string GetAccessToken(string code);

        Task<string> GetAccessTokenAsync(string code);

        string RefreshAccessToken();

        Task<string> RefreshAccessTokenAsync(string code);
    }
}
