using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
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


        public static ConfigurationType None => new ConfigurationType("None");
    }
}
