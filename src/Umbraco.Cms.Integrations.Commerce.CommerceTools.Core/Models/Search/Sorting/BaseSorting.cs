using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Sorting
{
    public abstract class BaseSorting<TSortingProperty> : ISorting
        where TSortingProperty : Enum
    {
        public TSortingProperty Property { get; }

        public SortingDirection Direction { get; }

        public BaseSorting(TSortingProperty property, SortingDirection direction)
        {
            Property = property;
            Direction = direction;
        }

        public abstract string Stringify();
    }
}
