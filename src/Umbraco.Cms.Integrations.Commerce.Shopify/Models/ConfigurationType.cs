namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models
{
    public class ConfigurationType
    {
        [JsonPropertyName("value")]
        public string Value { get; private set; }

        private ConfigurationType(string value)
        {
            Value = value;
        }

        public static ConfigurationType Api => new ConfigurationType("API");

        public static ConfigurationType OAuth => new ConfigurationType("OAuth");

        public static ConfigurationType OAuthConnected => new ConfigurationType("OAuthConnected");

        public static ConfigurationType None => new ConfigurationType("None");
    }
}
