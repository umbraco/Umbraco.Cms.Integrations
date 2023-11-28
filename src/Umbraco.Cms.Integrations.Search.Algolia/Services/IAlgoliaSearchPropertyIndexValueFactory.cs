using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaSearchPropertyIndexValueFactory
    {
        /// <summary>
        /// Get property indexed value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="culture"></param>
        /// <returns>[alias, value] pair</returns>
        KeyValuePair<string, object> GetValue(IProperty property, string culture);
    }
}
