
namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign
{
    public class Constants
    {
        public const string PropertyEditorAlias = "ActiveCampaign.FormPicker";

        public const string SettingsPath = "Umbraco:Cms:Integrations:Crm:ActiveCampaign:Settings";

        public const string FormsHttpClient = "FormsClient";

        public const int DefaultPageSize = 10;

        public class Resources
        {
            public const string AuthorizationFailed = "ActiveCampaign authorization failed.";

            public const string ApiAccessFailed = "Failed to connect to ActiveCampaign API.";
        }

        public static class ManagementApi
        {
            public const string RootPath = "active-campaign-forms/management/api";

            public const string ApiTitle = "ActiveCampaign Forms Management API";

            public const string ApiName = "active-campaign-management";

            public const string GroupName = "Active Campaign Forms";
        }
    }
}
