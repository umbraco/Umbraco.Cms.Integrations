#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
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
        public delegate IGoogleAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<GoogleSearchConsoleSettings>()
               .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            var oauthOptions = builder.Services.AddOptions<GoogleSearchConsoleOAuthSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.OAuthSettings));

            builder.ContentApps().Append<URLInspectionToolContentApp>();

            builder.Services.AddSingleton<ITokenService, TokenService>();

            builder.Services.AddTransient<UmbracoAuthorizationService>();
            builder.Services.AddTransient<AuthorizationService>();
            builder.Services.AddTransient<AuthorizationImplementationFactory>(f => useUmbracoAuthorization =>
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
            composition.ContentApps().Append<URLInspectionToolContentApp>();

            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);

            composition.Register<UmbracoAuthorizationService>(Lifetime.Transient);
            composition.Register<AuthorizationService>(Lifetime.Transient);
            composition.Register<AuthorizationImplementationFactory>(f => (useUmbracoAuthorization) =>
            {
                if (useUmbracoAuthorization)
                    return f.GetInstance<UmbracoAuthorizationService>();

                return f.GetInstance<AuthorizationService>();
            }, Lifetime.Transient);
        }
#endif

    }
}
