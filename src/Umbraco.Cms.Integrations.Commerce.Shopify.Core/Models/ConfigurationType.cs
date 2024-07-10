using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models
{
    public class ConfigurationType
    {
        [JsonProperty("value")]
        public string Value { get; private set; }

        private ConfigurationType(string value)
        {
            Value = value;
        }

        public static ConfigurationType Api => new ConfigurationType("API");

        public static ConfigurationType OAuth => new ConfigurationType("OAuth");


        public static ConfigurationType None => new ConfigurationType("None");
    }
}
