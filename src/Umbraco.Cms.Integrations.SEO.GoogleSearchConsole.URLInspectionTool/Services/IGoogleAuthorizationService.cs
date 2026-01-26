using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public interface IGoogleAuthorizationService
    {
        string GetAuthorizationUrl();

        string GetAccessToken(string code);

        Task<GoogleSearchConsoleResult> GetAccessTokenAsync(string code);

        string RefreshAccessToken();

        Task<GoogleSearchConsoleResult> RefreshAccessTokenAsync();
    }
}
