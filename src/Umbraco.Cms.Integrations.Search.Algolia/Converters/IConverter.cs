namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    /// <summary>
    /// Defines a custom index converter.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Gets the name of the converter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parses the index value.
        /// </summary>
        object ParseIndexValue(KeyValuePair<string, IEnumerable<object>> indexValue);
    }
}
