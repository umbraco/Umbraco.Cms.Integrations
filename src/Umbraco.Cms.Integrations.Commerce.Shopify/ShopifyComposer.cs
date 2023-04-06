using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

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
        public delegate IShopifyAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<ShopifySettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            var oauthOptions = builder.Services.AddOptions<ShopifyOAuthSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.OAuthSettings));

            builder.Services.AddSingleton<ITokenService, TokenService>();

            builder.Services.AddSingleton<ICacheHelper, CacheHelper>();

            builder.Services.AddSingleton<IShopifyService, ShopifyService>();

            builder.Services.AddSingleton<UmbracoAuthorizationService>();
            builder.Services.AddSingleton<AuthorizationService>();
            builder.Services.AddSingleton<AuthorizationImplementationFactory>(f => useUmbracoAuthorization =>
            {
                return useUmbracoAuthorization switch
                {
                    true => f.GetService<UmbracoAuthorizationService>(),
                    _ => f.GetService<AuthorizationService>()
                };
            });
        }
#else
        public void Compose(Composition composition)
        {
            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);

            composition.Register<ICacheHelper, CacheHelper>(Lifetime.Singleton);

            composition.Register<IShopifyService, ShopifyService>(Lifetime.Singleton);

            composition.Register<UmbracoAuthorizationService>(Lifetime.Singleton);
            composition.Register<AuthorizationService>(Lifetime.Singleton);
            composition.Register<AuthorizationImplementationFactory>(f => (useUmbracoAuthorization) =>
            {
                if (useUmbracoAuthorization)
                    return f.GetInstance<UmbracoAuthorizationService>();

                return f.GetInstance<AuthorizationService>();
            }, Lifetime.Singleton);
        }
#endif
    }
}
