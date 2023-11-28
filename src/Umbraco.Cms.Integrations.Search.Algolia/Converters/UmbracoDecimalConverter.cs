namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoDecimalConverter : IConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Decimal;

        public object ParseIndexValue(KeyValuePair<string, IEnumerable<object>> indexValue)
        {
            if (indexValue.Value != null && indexValue.Value.Any())
            {
                var value = indexValue.Value.FirstOrDefault();

                return value != null
                    ? decimal.Parse(value.ToString())
                    : default;
            }

            return default(decimal);
        }
    }
}
