
namespace Umbraco.Cms.Integrations.Crm.Dynamics
{
    public class Constants
    {
        public const string DynamicsOAuthConfigurationTable = "dynamicsOAuthConfiguration";

        public const string MigrationPlanName = "DynamicsOAuthMigrationPlan";

        public const string TargetStateName = "dynamicsOAuthConfiguration-db";

        public const string AlterAccessTokenColumnLengthTargetStateName = "dynamicsOAuthConfiguration-alter-access-token-column-length-db";

        public const int AccessTokenFieldSize = 4000;

        public static class RenderingComponent
        {
            public const string DefaultViewPath = "~/Views/Dynamics/Render/DynamicsForm.cshtml";
        }

        public static class ManagementApi
        {
            public const string RootPath = "dynamics/management/api";

            public const string ApiName = "dynamics-management";

            public const string ApiTitle = "Dynamics Management API";

            public const string GroupName = "Dynamics";
        }

        public static class EmbedAttribute
        {
            public const string DataFormBlockId = "data-form-block-id";

            public const string ContainerId = "id";

            public const string ContainerClass = "class";

            public const string DataWebsiteId = "data-website-id";

            public const string DataHostname = "data-hostname";
        }

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:Crm:Dynamics:Settings";

            public const string OAuthSettings = "Umbraco:Cms:Integrations:Crm:Dynamics:OAuthSettings";
        }

        public static class Modules
        {
            public const string OutboundPath = "msdyncrm_marketingforms";

            public const string RealTimePath = "msdynmkt_marketingforms";
        }
    }
}
