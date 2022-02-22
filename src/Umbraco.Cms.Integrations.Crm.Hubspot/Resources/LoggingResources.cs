
namespace Umbraco.Cms.Integrations.Crm.Hubspot.Resources
{
    public static class LoggingResources
    {
        public const string ApiKeyMissing = "Cannot access Hubspot - API key is missing";

        public const string ApiFetchFormsFailed =
            "Failed to fetch forms from Hubspot using API key: {0}";

        public const string OAuthFetchFormsFailed = "Failed to fetch forms from Hubspot using OAuth: {0}";

        public const string AccessTokenMissing = "Cannot access Hubspot - Access Token is missing.";
    }
}
