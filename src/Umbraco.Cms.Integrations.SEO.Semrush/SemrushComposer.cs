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

            builder.Services.AddScoped<TokenBuilder>();
        }
#else
 public void Compose(Composition composition)
        {
            composition.ContentApps().Append<SemrushContentApp>();

            composition.Register<TokenBuilder>(Lifetime.Request);
        }
#endif
    }
}
