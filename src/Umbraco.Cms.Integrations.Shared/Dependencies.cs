using Umbraco.Cms.Integrations.Shared.Services;

#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
#else
using Umbraco.Core.Composing;
using Umbraco.Core;
#endif

namespace Umbraco.Cms.Integrations.Shared
{
    public class Dependencies : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ITokenService, TokenService>();

            builder.Services.AddSingleton<ICacheHelper, CacheHelper>();
        }
#else
 public void Compose(Composition composition)
        {
            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);

            composition.Register<ICacheHelper, CacheHelper>(Lifetime.Singleton);
        }
#endif
    }
}
