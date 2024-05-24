using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoDecimalConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Decimal;

        public object ParseIndexValues(IProperty property) =>
            property.TryGetPropertyIndexValue(out string value)
            ? (decimal.TryParse(value.ToString(), out var result)
                ? result
                : default)
            : default;
    }
}
