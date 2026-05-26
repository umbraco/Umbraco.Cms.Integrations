using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign
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
                            $"{builder.Config.GetSection(Constants.SettingsPath)[nameof(ActiveCampaignSettings.BaseUrl)]}");
                        client.DefaultRequestHeaders
                            .Add("Api-Token", builder.Config.GetSection(Constants.SettingsPath)[nameof(ActiveCampaignSettings.ApiKey)]);
                    });

            builder.AddBackOfficeOpenApiDocument(
                Constants.ManagementApi.ApiName,
                document => document
                    .WithTitle(Constants.ManagementApi.ApiTitle)
                    .WithBackOfficeAuthentication()
                    .ConfigureOpenApiOptions(openApiOptions => openApiOptions.AddDocumentTransformer((doc, _, _) =>
                    {
                        doc.Info.Version = "Latest";
                        doc.Info.Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling Active Campaign product(s) and configuration.";
                        return Task.CompletedTask;
                    })));
        }

    }
}
