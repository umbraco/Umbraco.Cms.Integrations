using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Search.Sorting
{
    public class ProductSorting : BaseSorting<ProductSortingProperty>
    {
        public string LanguageCode { get; }

        public ProductSorting(ProductSortingProperty property, SortingDirection direction, string languageCode) : base(property, direction)
        {
            LanguageCode = languageCode;
        }

        public override string Stringify()
        {
            var propertyString = Property == ProductSortingProperty.Key ? "key" : Property == ProductSortingProperty.Id ? "id" : Property == ProductSortingProperty.Name ? $"masterData.current.name.{LanguageCode}" : null;

            var directionString = Direction == SortingDirection.Ascending ? "asc" : Direction == SortingDirection.Descending ? "desc" : null;

            return propertyString == null || directionString == null
                ? null
                : $"{propertyString} {directionString}";
        }
    }
}
