using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoTagsConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Tags;

        public object ParseIndexValues(IProperty property, IndexValue indexValue)
        {
            if (indexValue != null && indexValue.Values.Any())
            {
                return indexValue.Values;
            }

            return Enumerable.Empty<string>();
        }
    }
}
