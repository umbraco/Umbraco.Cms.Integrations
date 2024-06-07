
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

            public const string UmbracoCmsIntegrationsSeoSemrushBaseUrlKey =
                "Umbraco.Cms.Integrations.SEO.Semrush.BaseUrl";

            public const string UmbracoCmsIntegrationsSeoSemrushUseUmbracoAuthorizationKey =
                "Umbraco.Cms.Integrations.SEO.Semrush.UseUmbracoAuthorization";

            public const string UmbracoCmsIntegrationsSeoSemrushRefKey = "Umbraco.Cms.Integrations.SEO.Semrush.Ref";

            public const string UmbracoCmsIntegrationsSeoSemrushClientIdKey = "Umbraco.Cms.Integrations.SEO.Semrush.ClientId";

            public const string UmbracoCmsIntegrationsSeoSemrushClientSecretKey = "Umbraco.Cms.Integrations.SEO.Semrush.ClientSecret";

            public const string UmbracoCmsIntegrationsSeoSemrushRedirectUriKey = "Umbraco.Cms.Integrations.SEO.Semrush.RedirectUri";

            public const string UmbracoCmsIntegrationsSeoSemrushScopesKey = "Umbraco.Cms.Integrations.SEO.Semrush.Scopes";

            public const string UmbracoCmsIntegrationsSeoSemrushTokenEndpointKey = "Umbraco.Cms.Integrations.SEO.Semrush.TokenEndpoint";
        }
    }
}
