
namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Models.Dtos
{
    public class ApiAccessDto
    {
        private readonly string _baseUrl;

        private readonly string _apiKey;

        public ApiAccessDto(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        public string Account => IsApiConfigurationValid
            ? _baseUrl.Substring(0, _baseUrl.IndexOf(".")).Replace("https://", string.Empty)
            : string.Empty;

        public bool IsApiConfigurationValid => !string.IsNullOrEmpty(_baseUrl) && !string.IsNullOrEmpty(_apiKey);
    }
}
