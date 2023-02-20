
namespace Umbraco.Cms.Integrations.DAM.Aprimo
{
    public class Constants
    {
        public const string PropertyEditorAlias = "Umbraco.Cms.Integrations.DAM.Aprimo.MediaPicker";

        public const string SettingsPath = "Umbraco:Cms:Integrations:DAM:Aprimo:Settings";

        public const string AprimoClient = "AprimoHttpClient";

        public class Migration
        {
            public const string Name = "AprimoOAuthMigrationPlan";

            public const string TableName = "aprimoOAuthConfiguration";

            public const string TargetStateName = "aprimoOAuthConfiguration-db";
        }

        public class ErrorResources
        {
            public const string InvalidApiConfiguration = "API Configuration is not valid.";

            public const string InvalidAuthorizationResponse = "Invalid Aprimo authorization response.";

            public const string InvalidCodeChallenge = "Invalid code challenge.";

            public const string Unauthorized = "Invalid access token.";

            public const string MissingRefreshToken = "No refresh token could be found.";

            public const string RetrieveAccessToken = "Failed to retrieve an access token.";
        }

    }
}
