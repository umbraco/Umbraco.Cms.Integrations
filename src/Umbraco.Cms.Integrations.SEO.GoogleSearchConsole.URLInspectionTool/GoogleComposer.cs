using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Api.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool;

public class GoogleComposer : IComposer
{
    public delegate IGoogleAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

    public void Compose(IUmbracoBuilder builder)
    {
        var options = builder.Services.AddOptions<GoogleSearchConsoleSettings>()
           .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
        var oauthOptions = builder.Services.AddOptions<GoogleSearchConsoleOAuthSettings>()
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

        // Generate Swagger documentation for Shopify API
        builder.Services.Configure<SwaggerGenOptions>(options =>
        {
            options.SwaggerDoc(
                Constants.ManagementApi.ApiName,
                new OpenApiInfo
                {
                    Title = Constants.ManagementApi.ApiTitle,
                    Version = "Latest",
                    Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling GoogleSearchConsole and configuration."
                });
            options.OperationFilter<BackOfficeSecurityRequirementsOperationFilter>();
        })
        .AddSingleton<IOperationIdHandler, GoogleOperationIdHandler>();
    }
}
