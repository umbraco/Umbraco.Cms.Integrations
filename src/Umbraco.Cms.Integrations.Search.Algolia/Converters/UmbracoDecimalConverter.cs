using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoDecimalConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Decimal;

        public object ParseIndexValues(IProperty property, IEnumerable<object> indexValues)
        {
            if (indexValues != null && indexValues.Any())
            {
                var value = indexValues.FirstOrDefault();

                return value != null
                    ? decimal.Parse(value.ToString())
                    : default;
            }

            return default(decimal);
        }
    }
}
