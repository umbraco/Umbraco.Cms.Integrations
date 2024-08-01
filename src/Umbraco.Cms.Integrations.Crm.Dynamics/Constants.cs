
namespace Umbraco.Cms.Integrations.Crm.Dynamics
{
    public class Constants
    {
        public const string PropertyEditorAlias = "Dynamics.FormPicker";

        public const string DynamicsOAuthConfigurationTable = "dynamicsOAuthConfiguration";

        public const string MigrationPlanName = "DynamicsOAuthMigrationPlan";

        public const string TargetStateName = "dynamicsOAuthConfiguration-db";

        public const string AlterAccessTokenColumnLengthTargetStateName = "dynamicsOAuthConfiguration-alter-access-token-column-length-db";

        public const int AccessTokenFieldSize = 4000;

        public const string AppPluginFolderPath = "~/App_Plugins/Dynamics";

        public static class RenderingComponent
        {
            public const string DefaultViewPath = AppPluginFolderPath + "/Render/DynamicsForm.cshtml";
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

            public const string UmbracoCmsIntegrationsCrmDynamicsHostUrlKey = "Umbraco.Cms.Integrations.Crm.Dynamics.HostUrl";

            public const string UmbracoCmsIntegrationsCrmDynamicsApiPathKey = "Umbraco.Cms.Integrations.Crm.Dynamics.ApiPath";

            public const string UmbracoCmsIntegrationsCrmDynamicsUseUmbracoAuthorizationKey = 
                "Umbraco.Cms.Integrations.Crm.Dynamics.UseUmbracoAuthorization";

            public const string UmbracoCmsIntegrationsCrmDynamicsClientIdKey = "Umbraco.Cms.Integrations.Crm.Dynamics.ClientId";

            public const string UmbracoCmsIntegrationsCrmDynamicsClientSecretKey = "Umbraco.Cms.Integrations.Crm.Dynamics.ClientSecret";

            public const string UmbracoCmsIntegrationsCrmDynamicsRedirectUriKey = "Umbraco.Cms.Integrations.Crm.Dynamics.RedirectUri";

            public const string UmbracoCmsIntegrationsCrmDynamicsScopesKey = "Umbraco.Cms.Integrations.Crm.Dynamics.Scopes";

            public const string UmbracoCmsIntegrationsCrmDynamicsTokenEndpointKey = "Umbraco.Cms.Integrations.Crm.Dynamics.TokenEndpoint";
        }

        public static class Modules
        {
            public const string OutboundPath = "msdyncrm_marketingforms";

            public const string RealTimePath = "msdynmkt_marketingforms";
        }
    }
}
