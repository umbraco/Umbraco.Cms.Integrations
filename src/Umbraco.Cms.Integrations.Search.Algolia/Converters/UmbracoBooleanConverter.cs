using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoBooleanConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Boolean;

        public object ParseIndexValues(IProperty property, IEnumerable<object> indexValues)
        {
            if (indexValues != null && indexValues.Any())
            {
                var value = indexValues.FirstOrDefault();

                return value != null
                    ? value.Equals(1)
                    : default;
            }

            return default(bool);
        }
    }
}
