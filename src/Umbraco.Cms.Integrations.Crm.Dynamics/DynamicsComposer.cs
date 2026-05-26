global using System.Text.Json;
global using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Migrations;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

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

            builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, UmbracoAppStartingHandler>();

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

            builder.AddBackOfficeOpenApiDocument(
                Constants.ManagementApi.ApiName,
                document => document
                    .WithTitle(Constants.ManagementApi.ApiTitle)
                    .WithBackOfficeAuthentication()
                    .ConfigureOpenApiOptions(openApiOptions => openApiOptions.AddDocumentTransformer((doc, _, _) =>
                    {
                        doc.Info.Version = "Latest";
                        doc.Info.Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling Dynamics forms and configuration.";
                        return Task.CompletedTask;
                    })));
        }

    }
}