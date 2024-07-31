using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoBooleanConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Boolean;

        public object ParseIndexValues(IProperty property) =>
            property.TryGetPropertyIndexValue(out string value)
                ? value.Equals("1")
                : default;

    }
}
