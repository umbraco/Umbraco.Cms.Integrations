using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaSearchPropertyIndexValueFactory
    {
        Dictionary<string, Func<KeyValuePair<string, IEnumerable<object>>, object>> Converters { get; set; }
        
        /// <summary>
        /// Get property indexed value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="culture"></param>
        /// <returns>[alias, value] pair</returns>
        KeyValuePair<string, object> GetValue(IProperty property, string culture);
        /// <summary>
        /// Get property indexed value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="culture"></param>
        /// <returns>[alias, value] pair</returns>
        KeyValuePair<string, object> GetValue(IPublishedProperty property, string culture);
    }
}
