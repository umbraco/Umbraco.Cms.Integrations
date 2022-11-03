
namespace Umbraco.Cms.Integrations.Automation.Zapier
{
    public class Constants
    {
        public const string ZapierSubscriptionHookTable = "zapierSubscriptionHook";

        public const string MigrationPlanName = "ZapierMigrationPlan";

        public const string TargetStateName = "zapiersubscriptionhook-db";

        public const string UmbracoCmsIntegrationsAutomationZapierUserGroupAlias = "Umbraco.Cms.Integrations.Automation.Zapier.UserGroupAlias";

        public const string UmbracoCmsIntegrationsAutomationZapierApiKey = "Umbraco.Cms.Integrations.Automation.Zapier.ApiKey";

        public static class ZapierAppConfiguration
        {
            public const string UsernameHeaderKey = "X-USERNAME";

            public const string PasswordHeaderKey = "X-PASSWORD";

            public const string ApiKeyHeaderKey = "X-APIKEY";
        }

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:Automation:Zapier:Settings";
        }

        public static class ContentProperties
        {
            public const string Id = "id";

            public const string Name = "name";

            public const string PublishDate = "publishDate";
        }

        public static class EntityType
        {
            public const int Content = 1;

            public const int Form = 2;
        }
    }
}
