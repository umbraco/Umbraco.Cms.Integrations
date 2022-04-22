
using Microsoft.Extensions.DependencyInjection;
#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;

#else
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
#endif

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool
{
    public class GoogleComposer : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ContentApps().Append<URLInspectionToolContentApp>();

            builder.Services.AddSingleton<GoogleService>();

            builder.Services.AddSingleton<ITokenService, TokenService>();
        }
#else
        public void Compose(Composition composition)
        {
            composition.ContentApps().Append<URLInspectionToolContentApp>();

            composition.Register<GoogleService>(Lifetime.Singleton);

            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);
        }
#endif
     
    }
}
