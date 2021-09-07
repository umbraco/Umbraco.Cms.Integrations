using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Filters
{
    public class IdFilter : BaseFilter
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Filter for a specific ID.
        /// </summary>
        /// <param name="id"></param>
        public IdFilter(Guid id)
        {
            Id = id;
        }

        public override string Stringify()
        {
            return $"id=\"{Id}\"";
        }
    }
}
