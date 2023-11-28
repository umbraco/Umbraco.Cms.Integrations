namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoBooleanConverter : IConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Boolean;

        public object ParseIndexValue(KeyValuePair<string, IEnumerable<object>> indexValue)
        {
            if (indexValue.Value != null && indexValue.Value.Any())
            {
                var value = indexValue.Value.FirstOrDefault();

                return value != null
                    ? value.Equals(1)
                    : default;
            }

            return default(bool);
        }
    }
}
