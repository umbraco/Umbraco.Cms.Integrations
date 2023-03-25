#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Crm.Dynamics.Migrations;
#else
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics
{
    public class DynamicsComposer : IComposer
    {
        public delegate IDynamicsAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<DynamicsSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            var oauthOptions = builder.Services.AddOptions<DynamicsOAuthSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.OAuthSettings));

            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoAppStartingHandler>();

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

            builder.Services.AddSingleton<DynamicsService>();

            builder.Services.AddSingleton<DynamicsConfigurationService>();
        }
#else
        public void Compose(Composition composition)
        {
            composition.Register<UmbracoAuthorizationService>(Lifetime.Singleton);
            composition.Register<AuthorizationService>(Lifetime.Singleton);
            composition.Register<AuthorizationImplementationFactory>(f => (useUmbracoAuthorization) =>
            {
                if (useUmbracoAuthorization)
                    return f.GetInstance<UmbracoAuthorizationService>();

                return f.GetInstance<AuthorizationService>();
            }, Lifetime.Singleton);

            composition.Register<DynamicsService>(Lifetime.Singleton);

            composition.Register<DynamicsConfigurationService>(Lifetime.Singleton);
        }
#endif

    }
}
