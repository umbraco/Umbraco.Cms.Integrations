using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos
{
    public class ApiAccessDto
    {
        private string _baseUrl { get; set; }

        private string _apiKey { get; set; }

        public ApiAccessDto(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        [JsonProperty("account")]
        public string Account => IsApiConfigurationValid
            ? _baseUrl.Substring(0, _baseUrl.IndexOf(".")).Replace("https://", string.Empty)
            : string.Empty;

        [JsonProperty("isApiConfigurationValid")]
        public bool IsApiConfigurationValid => !string.IsNullOrEmpty(_baseUrl) && !string.IsNullOrEmpty(_apiKey);
    }
}
