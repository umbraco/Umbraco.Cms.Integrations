using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaSearchPropertyIndexValueFactory
    {
        Dictionary<string, Func<KeyValuePair<string, IEnumerable<object>>, string>> Converters { get; set; }
        
        /// <summary>
        /// Get property indexed value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="culture"></param>
        /// <returns>[alias, value] pair</returns>
        KeyValuePair<string, string> GetValue(IProperty property, string culture);
    }
}
