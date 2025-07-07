
namespace Umbraco.Cms.Integrations.Search.Algolia
{
    public class Constants
    {
        public const string SettingsPath = "Umbraco:Cms:Integrations:Search:Algolia:Settings";

        public const string AlgoliaIndicesTableName = "algoliaIndices";

        public static class ManagementApi
        {
            public const string RootPath = "algolia-search/management/api";

            public const string ApiTitle = "Algolia Search Management API";

            public const string ApiName = "algolia-search-management";

            public const string GroupName = "AlgoliaSearch";
        }

        public static class OperationIds
        {
            public const string BuildSearchIndex = "BuildSearchIndex";

            public const string DeleteSearchIndex = "DeleteSearchIndex";

            public const string GetContentTypes = "GetContentTypes";

            public const string GetSearchIndexById = "GetSearchIndexById";

            public const string GetIndices = "GetIndices";

            public const string SaveIndex = "SaveIndex";

            public const string SearchIndex = "SearchIndex";
        }
    }
}
