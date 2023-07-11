using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;

#else
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier
{
    public class ZapierComposer : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services
                .AddOptions<ZapierSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            builder.Services
                .AddOptions<ZapierFormsSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.FormsSettings));

            builder
                .AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoAppStartingHandler>();

            builder.Services.AddSingleton<ZapierSubscriptionHookService>();

            builder.Services.AddScoped<ZapierService>();

            builder.Services.AddScoped<IUserValidationService, UserValidationService>();
        }
#else
        public void Compose(Composition composition)
        {
            composition.Register<ZapierSubscriptionHookService>(Lifetime.Singleton);

            composition.Register<ZapierService>(Lifetime.Singleton);

            composition.Register<IUserValidationService, UserValidationService>(Lifetime.Scope);
        }
#endif

    }
}
