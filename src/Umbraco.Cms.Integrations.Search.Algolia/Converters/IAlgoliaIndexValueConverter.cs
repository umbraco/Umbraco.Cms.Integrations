using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    /// <summary>
    /// Defines a custom index converter.
    /// </summary>
    public interface IAlgoliaIndexValueConverter
    {
        /// <summary>
        /// Gets the name of the converter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parses the index values.
        /// </summary>
        object ParseIndexValues(IProperty property, IndexValue indexValue);
    }
}
