using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Search.Filters
{
    public abstract class BaseFilter
    {
        /// <summary>
        /// Returns a URL query parameter string, representing the object.
        /// </summary>
        /// <remarks>URL encoding should not be done in this method, as this is done by the <see cref="CommercetoolsClient"/>.</remarks>
        /// <returns></returns>
        public abstract string Stringify();

        /// <summary>
        /// Allows for implicit conversion from string to <see cref="CustomFilter"/>, allowing for filters to be expressed as a string.
        /// </summary>
        /// <param name="filterText"></param>
        public static implicit operator BaseFilter(string filterText) => new CustomFilter(filterText);
    }
}
