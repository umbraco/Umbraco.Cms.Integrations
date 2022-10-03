namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Globalization;

    public static class Constants
    {
        public const string PropertyEditorAlias = "Umbraco.Cms.Integrations.Crm.Hubspot.FormPicker";

        public const string UmbracoCmsIntegrationsCrmHubspotApiKey = "Umbraco.Cms.Integrations.Crm.Hubspot.ApiKey";

        public const string UmbracoCmsIntegrationsCrmHubspotRegion = "Umbraco.Cms.Integrations.Crm.Hubspot.Region";

        public static class Configuration
        {
            public const string Settings = "Umbraco:Cms:Integrations:Crm:Hubspot:Settings";
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
