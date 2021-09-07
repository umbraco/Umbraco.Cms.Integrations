using Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Search.Sorting;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;

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
