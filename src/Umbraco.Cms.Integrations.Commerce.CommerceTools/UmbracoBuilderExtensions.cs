#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;
using System;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Configuration;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.NotificationHandlers;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Services;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddCommerceTools(this IUmbracoBuilder builder, Action<CommerceToolsSettings> defaultOptions = default)
        {

            // load up the settings.
            var options = builder.Services.AddOptions<CommerceToolsSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));

            if (defaultOptions != default)
            {
                options.Configure(defaultOptions);
            }

            options.ValidateDataAnnotations();

            builder.Services.AddSingleton<ICommerceToolsService, CommerceToolsService>();

            builder.AddNotificationHandler<ServerVariablesParsingNotification, AddApiBaseUrl>();

            return builder;
        }
    }
}
#endif