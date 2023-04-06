namespace Umbraco.Cms.Integrations.Crm.Hubspot
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

            public const string UmbracoCmsIntegrationsCrmHubspotApiKey = "Umbraco.Cms.Integrations.Crm.Hubspot.ApiKey";

            public const string UmbracoCmsIntegrationsCrmHubspotRegion = "Umbraco.Cms.Integrations.Crm.Hubspot.Region";

            public const string UmbracoCmsIntegrationsCrmHubspotUseUmbracoAuthorizationKey =
                "Umbraco.Cms.Integrations.Crm.Hubspot.UseUmbracoAuthorization";

            public const string UmbracoCmsIntegrationsCrmHubspotClientIdKey = "Umbraco.Cms.Integrations.Crm.Hubspot.ClientId";

            public const string UmbracoCmsIntegrationsCrmHubspotClientSecretKey = "Umbraco.Cms.Integrations.Crm.Hubspot.ClientSecret";

            public const string UmbracoCmsIntegrationsCrmHubspotRedirectUriKey = "Umbraco.Cms.Integrations.Crm.Hubspot.RedirectUri";

            public const string UmbracoCmsIntegrationsCrmHubspotScopesKey = "Umbraco.Cms.Integrations.Crm.Hubspot.Scopes";

            public const string UmbracoCmsIntegrationsCrmHubspotTokenEndpointKey = "Umbraco.Cms.Integrations.Crm.Hubspot.TokenEndpoint";
        }

        public static class ErrorMessages
        {
            public const string TokenPermissions = "Token does not have the required permissions.";

            public const string InvalidApiKey = "Invalid API key.";

            public const string ApiKeyMissing = "Cannot access Hubspot - API key is missing";

            public const string AccessTokenMissing = "Cannot access Hubspot - Access Token is missing.";

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
