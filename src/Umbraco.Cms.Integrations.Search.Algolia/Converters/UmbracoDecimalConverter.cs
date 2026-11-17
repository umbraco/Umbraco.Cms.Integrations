using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoDecimalConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Decimal;

        public object ParseIndexValues(IProperty property, string? culture)
        {
            var value = property.GetValue(culture, null, true);

            return value != null
                ? decimal.Parse(value.ToString()!)
                : default(decimal);
        }
    }
}
