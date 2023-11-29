using System.Text.Json;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoTagsConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Tags;

        public object ParseIndexValues(IEnumerable<object> indexValues)
        {
            if (indexValues != null && indexValues.Any())
            {
                return indexValues;
            }

            return Enumerable.Empty<string>();
        }
    }
}
