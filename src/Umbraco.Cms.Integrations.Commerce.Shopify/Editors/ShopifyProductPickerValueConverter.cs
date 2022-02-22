using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels;
using Umbraco.Cms.Integrations.Shared.Services;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
{
    public class ShopifyProductPickerValueConverter : PropertyValueConverterBase
    {
        private readonly IApiService<ProductsListDto> _apiService;

        public ShopifyProductPickerValueConverter(IApiService<ProductsListDto> apiService)
        {
            _apiService = apiService;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) =>
            propertyType.EditorAlias == Constants.PropertyEditors.ProductPickerAlias;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(List<ProductViewModel>);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) =>
            PropertyCacheLevel.Snapshot;

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source,
            bool preview)
        {
            if (source == null) return null;

            return source.ToString()
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToArray();
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType,
            PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null) return null;

            var ids = (long[]) inter;

            var t = Task.Run(async () => await _apiService.GetResults());
            var result = t.Result;

            var products = from p in result.Result.Products
                where ids.Contains(p.Id)
                select new ProductViewModel
                {
                    Title = p.Title,
                    Body = p.Body,
                    Image = p.Image.Src
                };

            return products.ToList();
        }
    }
}
