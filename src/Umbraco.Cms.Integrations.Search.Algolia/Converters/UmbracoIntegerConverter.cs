using System.Windows.Markup;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoIntegerConverter : IConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Integer;

        public object ParseIndexValue(KeyValuePair<string, IEnumerable<object>> indexValue)
        {
            if (indexValue.Value != null && indexValue.Value.Any())
            {
                var value = indexValue.Value.FirstOrDefault();

                return value ?? default(int);
            }

            return default(int);
        }
    }
}
