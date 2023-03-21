#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Microsoft.Extensions.DependencyInjection;
#else
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    public class HubspotComposer : IComposer
    {
        public delegate IHubspotAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<HubspotSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            var oauthOptions = builder.Services.AddOptions<HubspotOAuthSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.OAuthSettings));

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
