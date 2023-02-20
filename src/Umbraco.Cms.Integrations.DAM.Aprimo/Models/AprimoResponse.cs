using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class AprimoResponse<T>
        where T : class
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("failure")]
        public bool Failure => !Success;

        [JsonPropertyName("isAuthorized")]
        public bool IsAuthorized { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        protected AprimoResponse(bool success, string error, bool isAuthorized = true, T data = null)
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
            IsAuthorized = isAuthorized;
            Data = data;
        }

        public static AprimoResponse<T> Ok(T data) => new(true, string.Empty, data: data);

        public static AprimoResponse<T> Fail(string message, bool isAuthorized = true) => new(false, message, isAuthorized: isAuthorized);
    }
}
