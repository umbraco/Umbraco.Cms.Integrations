namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    public static class Constants
    {
        public const string PropertyEditorAlias = "HubSpot.FormPicker";

        public const string AccessTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.AccessTokenDbKey";

        public const string RefreshTokenDbKey = "Umbraco.Cms.Integrations.Hubspot.RefreshTokenDbKey";

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:Crm:Hubspot:Settings";

            public const string OAuthSettings = "Umbraco:Cms:Integrations:Crm:Hubspot:OAuthSettings";
        }

        public static class ErrorMessages
        {
            public const string TokenPermissions = "Token does not have the required permissions.";

            public const string InvalidApiKey = "Invalid API key.";

            public const string ApiKeyMissing = "Cannot access HubSpot - API key is missing";

            public const string AccessTokenMissing = "Cannot access HubSpot - Access Token is missing.";

            public const string OAuthInvalidToken = "Unable to connect to HubSpot. Please review the settings of the form picker property's data type.";

            public const string OAuthFetchFormsConfigurationFailed = "Unable to retrieve the list of forms from HubSpot. Please review the settings of the form picker property's data type.";
        }

        public static class ManagementApi
        {
            public const string RootPath = "hubspot-forms/management/api";

            public const string ApiTitle = "HubSpot Forms Management API";

            public const string ApiName = "hubspot-forms-management";

            public const string GroupName = "Forms";
        }

        public static class OperationIdentifiers
        {
            public const string CheckConfiguration = "CheckConfiguration";

            public const string GetAccessToken = "GetAccessToken";

            public const string GetFormsByApiKey = "GetFormsByApiKey";

            public const string GetFormsOAuth = "GetFormsOAuth";

            public const string GetAuthorizationUrl = "GetAuthorizationUrl";

            public const string RefreshAccessToken = "RefreshAccessToken";

            public const string RevokeAccessToken = "RevokeAccessToken";

            public const string ValidateAccessToken = "ValidateAccessToken";
        }
    }
}
