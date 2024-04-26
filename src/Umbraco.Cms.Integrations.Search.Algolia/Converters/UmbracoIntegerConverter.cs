using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoIntegerConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Integer;

        public object ParseIndexValues(IProperty property, IEnumerable<object> indexValues)
        {
            if (indexValues != null && indexValues.Any())
            {
                var value = indexValues.FirstOrDefault();

                return value ?? default(int);
            }

            return default(int);
        }
    }
}
