
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class Result
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public bool Failure => !Success;

        protected Result(bool success, string error)
        {
            if (success && !string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("A succesful Result cannot have an error message.", error);
            }

            if (!success && string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("A failure Result must have an error message.", error);
            }

            Success = success;
            Error = error;
        }

        public static Result Ok() => new (true, string.Empty);

        public static Result Fail(string message) => new (false, message);

    }
}
