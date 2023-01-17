using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.PIM.Inriver.Configuration;
using Umbraco.Cms.Integrations.PIM.Inriver.Services;

namespace Umbraco.Cms.Integrations.PIM.Inriver
{
    public class InriverComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services
                .AddOptions<InriverSettings>()
                .Bind(builder.Config.GetSection(Constants.SettingsPath));

            builder.Services
                .AddHttpClient(Constants.InriverClient, client =>
                {
                    client.BaseAddress = new Uri($"{builder.Config.GetSection(Constants.SettingsPath)[nameof(InriverSettings.ApiBaseUrl)]}");
                    client.DefaultRequestHeaders
                        .Add("X-inRiver-APIKey", builder.Config.GetSection(Constants.SettingsPath)[nameof(InriverSettings.ApiKey)]);
                });

            builder.Services.AddSingleton<IInriverService, InriverService>();
        }
    }
}
