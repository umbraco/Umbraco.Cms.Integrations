using Microsoft.Extensions.DependencyInjection;

using System;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Configuration;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core
{
    public class ActiveCampaignComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<ActiveCampaignSettings>()
                .Bind(builder.Config.GetSection(Constants.SettingsPath));

            builder.Services
                .AddHttpClient(Constants.FormsHttpClient, client =>
                    {
                        client.BaseAddress = new Uri(
                            $"{builder.Config.GetSection(Constants.SettingsPath)[nameof(ActiveCampaignSettings.BaseUrl)]}/api/3/forms");
                        client.DefaultRequestHeaders
                            .Add("Api-Token", builder.Config.GetSection(Constants.SettingsPath)[nameof(ActiveCampaignSettings.ApiKey)]);
                    });

        }

    }
}
