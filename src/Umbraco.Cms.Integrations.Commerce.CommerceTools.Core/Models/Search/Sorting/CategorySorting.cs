using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Sorting
{
    public class CategorySorting : BaseSorting<CategorySortingProperty>
    {
        public string LanguageCode { get; }

        public CategorySorting(CategorySortingProperty property, SortingDirection direction, string languageCode) : base(property, direction)
        {
            LanguageCode = languageCode;
        }

        public override string Stringify()
        {

            var propertyString = Property == CategorySortingProperty.Id ? "id" : Property == CategorySortingProperty.Name ? $"name.{LanguageCode}" : null;

            var directionString = Direction == SortingDirection.Ascending ? "asc" : Direction == SortingDirection.Descending ? "desc" : null;

            return propertyString == null || directionString == null
                ? null
                : $"{propertyString} {directionString}";
        }
    }
}
