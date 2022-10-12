using Newtonsoft.Json;

using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Models.Dtos
{
    public class ResponseDto<T>
        where T : class
    {
        [JsonProperty("data")]
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonProperty("message")]
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
