
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Models
{
    public class ServiceResponse<T>
        where T : class
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("failure")]
        public bool Failure { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        protected ServiceResponse(bool success, string error, T data = null)
        {
            if (success && !string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("A succesful Response cannot have an error message.", error);
            }

            if (!success && string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("A failure Response must have an error message.", error);
            }

            Success = success;
            Error = error;
            Data = data;
        }

        public static ServiceResponse<T> Ok(T data) => new(true, string.Empty, data);

        public static ServiceResponse<T> Fail(string message) => new(false, message);
    }
}
