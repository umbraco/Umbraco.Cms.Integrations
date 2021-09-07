using Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Search.Sorting;

#if NETCOREAPP
using Umbraco.Cms.Core;

#else
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Extensions
{
    public static class DirectionExtensions
    {
        public static SortingDirection ToSortingDirection(this Direction direction)
        {
            if (direction is Direction.Ascending)
            {
                return SortingDirection.Ascending;
            }

            if (direction is Direction.Descending)
            {
                return SortingDirection.Descending;
            }

            return SortingDirection.None;
        }
    }
}
