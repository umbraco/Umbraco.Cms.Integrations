using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public interface IGoogleAuthorizationService
    {
        string GetAuthorizationUrl();

        string GetAccessToken(string code);

        Task<string> GetAccessTokenAsync(string code);

        string RefreshAccessToken();

        Task<string> RefreshAccessTokenAsync();
    }
}
