
namespace Umbraco.Cms.Integrations.SEO.Semrush
{
    public class Constants
    {
        public const int DefaultPageSize = 10;

        public const string TokenDbKey = "Umbraco.Cms.Integrations.Semrush.TokenDbKey";

        public const string AllowLimitOffsetHeaderName = "Allow-Limit-Offset";

        public const string SemrushKeywordsEndpoint = "{0}api/v1/keywords/{1}?access_token={2}&phrase={3}&database={4}";

        public const string BadRefreshToken = "BAD_REFRESH_TOKEN";

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:SEO:Semrush:Settings";

            public const string OAuthSettings = "Umbraco:Cms:Integrations:SEO:Semrush:OAuthSettings";
        }

        public static class ManagementApi
        {
            public const string RootPath = "semrush/management/api";

            public const string ApiTitle = "Semrush Management API";

            public const string ApiName = "semrush-management";

            public const string SemrushGroupName = "Semrush";

            public const string TokenName = "Access Token";
        }
    }
}
