namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public interface ISemrushAuthorizationService
    {
        string GetAuthorizationUrl();

        string GetAccessToken(string code);

        Task<string> GetAccessTokenAsync(string code);

        string RefreshAccessToken();

        Task<string> RefreshAccessTokenAsync();
    }
}
