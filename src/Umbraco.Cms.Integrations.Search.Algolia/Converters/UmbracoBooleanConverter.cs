using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoBooleanConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Boolean;

        public object ParseIndexValues(IProperty property, IndexValue indexValue)
        {
            if (indexValue != null && indexValue.Values.Any())
            {
                var value = indexValue.Values.FirstOrDefault();

                return value != null
                    ? value.Equals(1)
                    : default;
            }

            return default(bool);
        }
    }
}
