using System.Text.Json;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoTagsConverter : IConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Tags;

        public object ParseIndexValue(KeyValuePair<string, IEnumerable<object>> indexValue)
        {
            if (indexValue.Value != null && indexValue.Value.Any())
            {
                return indexValue.Value;
            }

            return Enumerable.Empty<string>();
        }
    }
}
