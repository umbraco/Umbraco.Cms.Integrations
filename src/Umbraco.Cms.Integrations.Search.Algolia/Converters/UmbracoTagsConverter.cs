using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoTagsConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Tags;

        public object ParseIndexValues(IProperty property, string? culture)
        {
            // Nothing stored indexes as an empty string, as it did before CMS 19.
            if (property.GetValue(culture, null, true) is not string raw || string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            // Tags are stored as a JSON array, or as a comma separated list for the legacy storage format.
            if (raw.DetectIsJson())
            {
                return JsonSerializer.Deserialize<string[]>(raw) ?? (object)string.Empty;
            }

            var tags = raw.Split(',', StringSplitOptions.RemoveEmptyEntries);

            return tags;
        }
    }
}
