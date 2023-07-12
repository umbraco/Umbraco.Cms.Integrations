using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    public class HubspotComposer : IComposer
    {
        public delegate IHubspotAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

        public void Compose(IUmbracoBuilder builder)
        {
            var options = builder.Services.AddOptions<HubspotSettings>()
                .Bind(builder.Config.GetSection(Constants.Configuration.Settings));
            var oauthOptions = builder.Services.AddOptions<HubspotOAuthSettings>()
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
        }
    }
}
