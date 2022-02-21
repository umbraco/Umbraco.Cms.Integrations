using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;
using Umbraco.Cms.Integrations.Shared.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Cms.Integrations.Commerce.Shopify
{
    public class ShopifyComposer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IApiService<ProductsListDto>, ShopifyService>(Lifetime.Singleton);
        }
    }
}
