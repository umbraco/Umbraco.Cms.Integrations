using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.DAM.Aprimo.Configuration;
using Umbraco.Cms.Integrations.DAM.Aprimo.Migrations;
using Umbraco.Cms.Integrations.DAM.Aprimo.Services;

namespace Umbraco.Cms.Integrations.DAM.Aprimo
{
    public class AprimoComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services
                .AddOptions<AprimoSettings>()
                .Bind(builder.Config.GetSection(Constants.SettingsPath));

            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoAppStartingHandler>();

            builder.Services.AddScoped<OAuthConfigurationStorage>();

            builder.Services.AddSingleton<IAprimoAuthorizationService, AprimoAuthorizationService>();
        }
    }
}
