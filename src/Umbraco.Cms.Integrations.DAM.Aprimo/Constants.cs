
namespace Umbraco.Cms.Integrations.DAM.Aprimo
{
    public class Constants
    {
        public const string PropertyEditorAlias = "Umbraco.Cms.Integrations.DAM.Aprimo.MediaPicker";

        public const string SettingsPath = "Umbraco:Cms:Integrations:DAM:Aprimo:Settings";

        public class Migration
        {
            public const string Name = "AprimoOAuthMigrationPlan";

            public const string TableName = "aprimoOAuthConfiguration";

            public const string TargetStateName = "aprimoOAuthConfiguration-db";
        }
    }
}
