using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Search.Sorting
{
    public interface ISorting
    {
        /// <summary>
        /// Return a string that represents the state of the sorting object.
        /// </summary>
        /// <returns></returns>
        string Stringify();
    }
}
