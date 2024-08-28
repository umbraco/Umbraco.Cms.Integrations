global using System.Text.Json;
global using System.Text.Json.Serialization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
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
                .AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoAppStartingHandler>();
            builder.AddNotificationHandler<ContentPublishedNotification, NewContentPublishedNotification>();


            builder.Services.AddSingleton<ZapierSubscriptionHookService>();

            builder.Services.AddScoped<ZapierService>();

            builder.Services.AddScoped<IUserValidationService, UserValidationService>();

            builder.Services.AddScoped<IZapierContentService, ZapierContentService>();

            builder.Services.AddScoped<IZapierContentFactory, ZapierContentFactory>();

            // Generate Swagger documentation for Zapier API
            builder.Services.Configure<SwaggerGenOptions>(options =>
            {
                options.SwaggerDoc(
                    Constants.ManagementApi.ApiName,
                    new OpenApiInfo
                    {
                        Title = Constants.ManagementApi.ApiTitle,
                        Version = "Latest",
                        Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling Zapier automation and configuration."
                    });
                // remove this as Swagger throws an ArgumentException: An item with the same key has already been added. Key: 401
                //options.OperationFilter<BackOfficeSecurityRequirementsOperationFilter>();
                options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
            });
        }
    }
}
