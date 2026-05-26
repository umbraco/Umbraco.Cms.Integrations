using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool;

public class GoogleComposer : IComposer
{
    public delegate IGoogleAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddOptions<GoogleSearchConsoleSettings>()
           .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
        builder.Services.AddOptions<GoogleSearchConsoleOAuthSettings>()
            .Bind(builder.Config.GetSection(Constants.Configuration.OAuthSettings));

        builder.Services.AddSingleton<ITokenService, TokenService>();

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

        builder.AddBackOfficeOpenApiDocument(
            Constants.ManagementApi.ApiName,
            document => document
                .WithTitle(Constants.ManagementApi.ApiTitle)
                .WithBackOfficeAuthentication()
                .ConfigureOpenApiOptions(openApiOptions => openApiOptions.AddDocumentTransformer((doc, _, _) =>
                {
                    doc.Info.Version = "Latest";
                    doc.Info.Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling GoogleSearchConsole and configuration.";
                    return Task.CompletedTask;
                })));
    }
}
