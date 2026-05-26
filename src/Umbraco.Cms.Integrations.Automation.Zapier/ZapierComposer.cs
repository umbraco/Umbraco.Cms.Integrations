global using System.Text.Json;
global using System.Text.Json.Serialization;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Automation.Zapier.Components;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier
{
    public class ZapierComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services
                .AddOptions<ZapierSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            builder.Services
                .AddOptions<ZapierFormsSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.FormsSettings));

            builder
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, UmbracoAppStartingHandler>();
            builder.AddNotificationHandler<ContentPublishedNotification, NewContentPublishedNotification>();


            builder.Services.AddSingleton<ZapierSubscriptionHookService>();

            builder.Services.AddScoped<ZapierService>();

            builder.Services.AddScoped<IUserValidationService, UserValidationService>();

            builder.Services.AddScoped<IZapierContentService, ZapierContentService>();

            builder.Services.AddScoped<IZapierContentFactory, ZapierContentFactory>();

            builder.AddBackOfficeOpenApiDocument(
                Constants.ManagementApi.ApiName,
                document => document
                    .WithTitle(Constants.ManagementApi.ApiTitle)
                    .WithBackOfficeAuthentication()
                    .ConfigureOpenApiOptions(options => options.AddDocumentTransformer((doc, _, _) =>
                    {
                        doc.Info.Version = "Latest";
                        doc.Info.Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling Zapier automation and configuration.";
                        return Task.CompletedTask;
                    })));
        }
    }
}
