
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;
using Umbraco.Cms.Integrations.Shared.Services;

#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;

#else
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Umbraco.Cms.Integrations.Commerce.Shopify
{
    public class ShopifyComposer : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<ShopifySettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));

            builder.Services.AddSingleton<IApiService<ProductsListDto>, ShopifyService>();
        }
#else
public void Compose(Composition composition)
        {
            composition.Register<IApiService<ProductsListDto>, ShopifyService>(Lifetime.Singleton);
        }
#endif
    }
}
