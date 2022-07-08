using Newtonsoft.Json;

namespace Umbraco.Integrations.Library.Dtos
{
    public class ErrorDto
    {
        [JsonProperty("isSuccessful")]
        public bool IsSuccessful => string.IsNullOrWhiteSpace(Error);

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
