
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Search.Algolia.Models
{
    public class Result
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public bool Failure => !Success;

        /// <summary>
        /// Records the operation completed without, described as "Name (id)".
        /// </summary>
        /// <remarks>
        /// An index build skips records Algolia refuses individually - typically because they
        /// exceed the account's maximum record size - and carries on. Those are reported here
        /// rather than as an error, because the build itself succeeded.
        /// </remarks>
        public IReadOnlyCollection<string> SkippedItems { get; set; } = Array.Empty<string>();

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

        /// <summary>
        /// The operation completed, but some records were skipped. See <see cref="SkippedItems"/>.
        /// </summary>
        public static Result Partial(IReadOnlyCollection<string> skippedItems) =>
            new (true, string.Empty) { SkippedItems = skippedItems };

        public static Result Fail(string message) => new (false, message);

    }
}
