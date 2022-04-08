
namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public abstract class BaseAuthService
    {
        public abstract string GetClientId();

        public abstract string GetAuthorizationUrl();

        public abstract string GetAuthProxyTokenEndpoint();
    }
}
