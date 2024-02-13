namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Globalization;

    public static class Constants
    {
        public const string PropertyEditorAlias = "Umbraco.Cms.Integrations.Crm.Hubspot.FormPicker";

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

        internal static readonly JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
