using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class GeolocProperties
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}
