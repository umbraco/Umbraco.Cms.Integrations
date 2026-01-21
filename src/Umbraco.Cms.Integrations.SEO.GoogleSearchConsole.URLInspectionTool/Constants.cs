
namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool;

public class Constants
{
    public const string TokenDbKey = "Umbraco.Cms.Integrations.GoogleSearchConsole.URLInspectionToolTokenDbKey";

    public const string RefreshTokenDbKey =
        "Umbraco.Cms.Integrations.GoogleSearchConsole.URLInspectionToolRefreshTokenDbKey";

    public static class Configuration
    {
        public const string Settings = "Umbraco:Cms:Integrations:SEO:GoogleSearchConsole:Settings";

        public const string OAuthSettings = "Umbraco:Cms:Integrations:SEO:GoogleSearchConsole:OAuthSettings";
    }

    public static class ManagementApi
    {
        public const string RootPath = "googlesearchconsole/management/api";

        public const string ApiTitle = "GoogleSearchConsole Management API";

        public const string ApiName = "googlesearchconsole-management";

        public const string GoogleSearchConsoleGroupName = "GoogleSearchConsole";

        public const string TokenName = "Access Token";
    }
}
