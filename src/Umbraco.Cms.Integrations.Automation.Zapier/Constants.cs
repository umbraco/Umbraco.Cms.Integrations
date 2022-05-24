
namespace Umbraco.Cms.Integrations.Automation.Zapier
{
    public class Constants
    {
        public const string ZapierSubscriptionHookTable = "zapierSubscriptionHook";

        public const string MigrationPlanName = "ZapierMigrationPlan";

        public const string TargetStateName = "zapiersubscriptionhook-db";

        public const string UmbracoCmsIntegrationsAutomationZapierUserGroup = "Umbraco.Cms.Integrations.Automation.Zapier.UserGroup";

        public static class ZapierAppConfiguration
        {
            public const string UsernameHeaderKey = "X-USERNAME";

            public const string PasswordHeaderKey = "X-PASSWORD";
        }

        public static class Configuration
        {
            public const string Settings = "Umbraco:Forms:Integrations:Automation:Zapier:Settings";
        }

        public static class Content
        {
            public const string Id = "id";

            public const string Name = "name";

            public const string PublishDate = "publishDate";
        }
    }
}
