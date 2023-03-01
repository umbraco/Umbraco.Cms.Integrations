using Microsoft.Extensions.DependencyInjection;

using System.Net.Http.Headers;

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

            builder.Services
                .AddOptions<AprimoOAuthSettings>()
                .Bind(builder.Config.GetSection(Constants.OAuthSettingsPath));

            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoAppStartingHandler>();

            builder.Services.AddSingleton<OAuthConfigurationStorage>();

            builder.Services.AddSingleton<IAprimoAuthorizationService, AprimoAuthorizationService>();

            builder.Services.AddSingleton<IAprimoService, AprimoService>();

            builder.Services
                .AddHttpClient(Constants.AprimoClient, client =>
                {
                    client.BaseAddress =
                        new Uri($"https://{builder.Config.GetSection(Constants.SettingsPath)[nameof(AprimoSettings.Tenant)]}.dam.aprimo.com/api/core/");
                    client.DefaultRequestHeaders.Add("API-VERSION", "1");
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Umbraco.Cms.Integrations.DAM.Aprimo", "1.0.0"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("select-record", "title,tag,thumbnail");
                });

            builder.Services
               .AddHttpClient(Constants.AprimoAuthClient, client =>
               {
                   client.BaseAddress =
                       new Uri($"https://{builder.Config.GetSection(Constants.SettingsPath)[nameof(AprimoSettings.Tenant)]}.aprimo.com/");
               });
        }
    }
}
