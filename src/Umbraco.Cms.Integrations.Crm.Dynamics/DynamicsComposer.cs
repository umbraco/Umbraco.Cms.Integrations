global using System.Text.Json;
global using System.Text.Json.Serialization;

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Crm.Dynamics.Migrations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Umbraco.Cms.Integrations.Crm.Dynamics
{
    public class DynamicsComposer : IComposer
    {
        public delegate IDynamicsAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

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

            builder.Services.AddSingleton<IDynamicsConfigurationStorage, DynamicsConfigurationStorage>();

            builder.Services.AddSingleton<IDynamicsService, DynamicsService>();

            // Generate Swagger documentation for Zapier API
            builder.Services.Configure<SwaggerGenOptions>(options =>
            {
                options.SwaggerDoc(
                    Constants.ManagementApi.ApiName,
                    new OpenApiInfo
                    {
                        Title = Constants.ManagementApi.ApiTitle,
                        Version = "Latest",
                        Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling Dynamics forms and configuration."
                    });
                options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
            });
        }

    }
}