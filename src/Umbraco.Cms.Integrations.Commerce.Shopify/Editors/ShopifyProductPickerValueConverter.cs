using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

#if NETCOREAPP
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
{
    public class ShopifyProductPickerValueConverter : PropertyValueConverterBase
    {
        private readonly IShopifyService _apiService;

        public ShopifyProductPickerValueConverter(IShopifyService apiService)
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
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToArray();
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType,
            PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null) return null;

            var ids = (long[])inter;

            var t = Task.Run(async () => await _apiService.GetProductsByIds(ids));

            var result = t.Result;

            var products = from p in result.Result.Products
                           select new ProductViewModel
                           {
                               Id = p.Id,
                               Title = p.Title,
                               Body = p.Body,
                               ProductImage = p.Image != null ? new ProductImage { Src = p.Image.Src, Alt = p.Image.Alt } : null,
                               Tags = p.Tags,
                               ProductType = p.ProductType,
                               PublishedScope = p.PublishedScope,
                               Handle = p.Handle,
                               Status = p.Status,
                               Variants = from v in p.Variants
                                          select new VariantViewModel
                                          {
                                              InventoryQuantity = v.InventoryQuantity,
                                              Position = v.Position,
                                              Price = v.Price,
                                              Sku = v.Sku,
                                              Taxable = v.Taxable
                                          }
                           };

            return products.ToList();
        }
    }
}
