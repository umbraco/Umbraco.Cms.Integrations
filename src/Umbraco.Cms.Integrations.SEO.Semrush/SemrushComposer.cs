using Umbraco.Cms.Integrations.SEO.Semrush.Services;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;

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
        public delegate ISemrushAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<SemrushSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            var oauthOptions = builder.Services.AddOptions<SemrushOAuthSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.OAuthSettings));

            builder.ContentApps().Append<SemrushContentApp>();

            builder.Services.AddSingleton<ISemrushTokenService, SemrushTokenService>();
            builder.Services.AddSingleton<ICacheHelper, CacheHelper>();

            builder.Services.AddScoped<TokenBuilder>();

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
            composition.ContentApps().Append<SemrushContentApp>();

            composition.Register<ISemrushTokenService, SemrushTokenService>(Lifetime.Singleton);
            composition.Register<ICacheHelper, CacheHelper>(Lifetime.Singleton);

            composition.Register<TokenBuilder>(Lifetime.Request);

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
