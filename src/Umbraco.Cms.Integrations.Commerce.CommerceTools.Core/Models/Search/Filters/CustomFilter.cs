using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Filters
{
    public class CustomFilter : BaseFilter
    {
        public string FilterText { get; }

        /// <summary>
        /// Filter with a custom filter string.
        /// </summary>
        /// <param name="filterText"></param>
        public CustomFilter(string filterText)
        {
            FilterText = filterText;
        }

        public override string Stringify() => FilterText;
    }
}
