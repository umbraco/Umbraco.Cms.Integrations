using Umbraco.Cms.Integrations.Commerce.Shopify.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Integrations.Commerce.Shopify.Configuration;

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

            builder.Services.AddSingleton<UmbracoAuthorizationService>();
            builder.Services.AddSingleton<AuthorizationService>();

            // Generate Swagger documentation for Shopify API
            builder.Services.Configure<SwaggerGenOptions>(options =>
            {
                options.SwaggerDoc(
                    Constants.ManagementApi.ApiName,
                    new OpenApiInfo
                    {
                        Title = Constants.ManagementApi.ApiTitle,
                        Version = "Latest",
                        Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling indices."
                    });

                options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
            });
        }
    }
}
