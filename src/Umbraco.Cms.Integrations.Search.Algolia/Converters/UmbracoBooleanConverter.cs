using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoBooleanConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Boolean;

        public object ParseIndexValues(IProperty property, string? culture)
        {
            var value = property.GetValue(culture, null, true);

            return value != null
                ? value.Equals(1)
                : default(bool);
        }
    }
}
