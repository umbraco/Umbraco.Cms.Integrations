global using System.Text.Json;
global using System.Text.Json.Serialization;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;
using Umbraco.Cms.Integrations.Commerce.Shopify.Services;

namespace Umbraco.Cms.Integrations.Commerce.Shopify
{
    public class ShopifyComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<ShopifySettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            var oauthOptions = builder.Services.AddOptions<ShopifyOAuthSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.OAuthSettings));

            builder.Services.AddSingleton<ITokenService, TokenService>();

            builder.Services.AddSingleton<ICacheHelper, CacheHelper>();

            builder.Services.AddSingleton<IShopifyService, ShopifyService>();
            builder.Services.AddSingleton<IShopifyAuthorizationService, UmbracoAuthorizationService>();

            builder.Services.AddSingleton<UmbracoAuthorizationService>();
            builder.Services.AddSingleton<AuthorizationService>();

            builder.AddBackOfficeOpenApiDocument(
                Constants.ManagementApi.ApiName,
                document => document
                    .WithTitle(Constants.ManagementApi.ApiTitle)
                    .WithBackOfficeAuthentication()
                    .ConfigureOpenApiOptions(openApiOptions => openApiOptions.AddDocumentTransformer((doc, _, _) =>
                    {
                        doc.Info.Version = "Latest";
                        doc.Info.Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling Shopify product(s) and configuration.";
                        return Task.CompletedTask;
                    })));
        }
    }
}
