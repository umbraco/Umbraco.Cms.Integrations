using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Models;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Services;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;


namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Editors
{
    public class CommerceToolsPickerValueConverter : PropertyValueConverterBase
    {
        private readonly ICommerceToolsService _commerceToolsService;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public CommerceToolsPickerValueConverter(ICommerceToolsService commerceToolsService, IVariationContextAccessor variationContextAccessor)
        {
            _commerceToolsService = commerceToolsService;
            _variationContextAccessor = variationContextAccessor;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
            => propertyType.EditorAlias == Constants.PropertyEditors.PickerAlias;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var isMultiPicker = IsMultiPicker(propertyType);
            var entityType = GetEntityType(propertyType);

            if (entityType == EntityType.Category)
            {
                return isMultiPicker ? typeof(IEnumerable<Category>) : typeof(Category);
            }
            else if (entityType == EntityType.Product)
            {
                return isMultiPicker ? typeof(IEnumerable<Product>) : typeof(Product);
            }

            return typeof(object);
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
            => PropertyCacheLevel.Snapshot;

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;

            var entityIds = source.ToString()
                .Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToArray();
            return entityIds;
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel cacheLevel, object source, bool preview)
        {
            if (source == null)
            {
                return null;
            }

            var entityIds = (Guid[])source;
            var entityType = GetEntityType(propertyType);
            var isMultiPicker = IsMultiPicker(propertyType);

            if (entityType == EntityType.Category)
            {
                var commerceToolsPicker = _commerceToolsService.GetCategoriesByIdsAsync(entityIds, _variationContextAccessor.VariationContext.Culture).GetAwaiter().GetResult();
                if (isMultiPicker)
                {
                    return commerceToolsPicker;
                }

                return commerceToolsPicker.FirstOrDefault();
            }
            else if (entityType == EntityType.Product)
            {
                var commerceToolsPicker = _commerceToolsService.GetProductsByIdsAsync(entityIds, _variationContextAccessor.VariationContext.Culture).GetAwaiter().GetResult();
                if (isMultiPicker)
                {
                    return commerceToolsPicker;
                }

                return commerceToolsPicker.FirstOrDefault();
            }

            return null;
        }

        private static bool IsMultiPicker(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<CommerceToolsPickerConfiguration>();
            var isMultiPicker = !config.ValidationLimit.Max.HasValue || config.ValidationLimit.Max.Value > 1;
            return isMultiPicker;
        }

        private static EntityType GetEntityType(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<CommerceToolsPickerConfiguration>();
            return (EntityType)Enum.Parse(typeof(EntityType), config.EntityType);
        }
    }
}
