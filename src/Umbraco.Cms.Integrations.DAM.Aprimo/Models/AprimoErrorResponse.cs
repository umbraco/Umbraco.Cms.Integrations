using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class AprimoErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message {  get; set; } = string.Empty;

        [JsonPropertyName("exceptionMessage")]
        public string ExceptionMessage { get; set; } = string.Empty;

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Message)
                ? Message : ExceptionMessage;
        }
    }
}
