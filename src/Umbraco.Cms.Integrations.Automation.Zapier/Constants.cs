
namespace Umbraco.Cms.Integrations.Automation.Zapier
{
    public class Constants
    {
        public const string ZapierSubscriptionHookTable = "zapierSubscriptionHook";

        public const string MigrationPlanName = "ZapierMigrationPlan";

        public const string TargetStateName = "zapiersubscriptionhook-db";

        public static class ManagementApi
        {
            public const string RootPath = "zapier/management/api";

            public const string ApiName = "zapier-management";

            public const string ApiTitle = "Zapier Management API";

            public const string GroupName = "Zapier";
        }

        public static class ZapierAppConfiguration
        {
            public const string UsernameHeaderKey = "X-USERNAME";

            public const string PasswordHeaderKey = "X-PASSWORD";

            public const string ApiKeyHeaderKey = "X-APIKEY";
        }

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:Automation:Zapier:Settings";

            public const string FormsSettings = "Umbraco:Forms:Integrations:Automation:Zapier:Settings";
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
