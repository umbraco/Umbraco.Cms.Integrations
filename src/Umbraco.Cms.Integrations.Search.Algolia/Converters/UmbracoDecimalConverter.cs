using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoDecimalConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Decimal;

        public object ParseIndexValues(IProperty property, IndexValue indexValue)
        {
            if (indexValue != null && indexValue.Values.Any())
            {
                var value = indexValue.Values.FirstOrDefault();

                return value != null
                    ? decimal.Parse(value.ToString())
                    : default;
            }

            return default(decimal);
        }
    }
}
