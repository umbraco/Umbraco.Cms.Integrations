using Umbraco.Cms.Integrations.SEO.Semrush.Services;

#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#else
using Umbraco.Core.Composing;
using Umbraco.Core;
using Umbraco.Web;
#endif

namespace Umbraco.Cms.Integrations.SEO.Semrush
{
    public class SemrushComposer : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ContentApps().Append<SemrushContentApp>();

            builder.Services.AddSingleton<ISemrushTokenService, SemrushTokenService>();
            builder.Services.AddSingleton<ICacheHelper, CacheHelper>();

            builder.Services.AddScoped<TokenBuilder>();
        }
#else
 public void Compose(Composition composition)
        {
            composition.ContentApps().Append<SemrushContentApp>();

            composition.Register<ISemrushTokenService, SemrushTokenService>(Lifetime.Singleton);
            composition.Register<ICacheHelper, CacheHelper>(Lifetime.Singleton);

            composition.Register<TokenBuilder>(Lifetime.Request);
        }
#endif
    }
}
