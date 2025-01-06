using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Editors
{
    public class ShopifyProductPickerValueConverter : PropertyValueConverterBase
    {
        private readonly ShopifySettings _settings;
        private readonly IShopifyService _apiService;

        public ShopifyProductPickerValueConverter(IOptions<ShopifySettings> options, IShopifyService apiService)
        {
            _settings = options.Value;
            _apiService = apiService;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) =>
            propertyType.EditorAlias == Constants.PropertyEditors.ProductPickerAlias;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(List<ProductViewModel>);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) =>
            _settings.PropertyCacheLevel;

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source,
            bool preview)
        {
            if (source == null) return null;

            return JsonSerializer.Deserialize<long[]>(source.ToString());
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
                               ProductImage = p.Image != null ? new ProductImageViewModel { Src = p.Image.Src, Alt = p.Image.Alt } : null,
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
