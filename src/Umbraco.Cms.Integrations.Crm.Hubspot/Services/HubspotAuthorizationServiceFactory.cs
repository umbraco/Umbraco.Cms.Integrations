using Microsoft.Extensions.DependencyInjection;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Services;

public class HubspotAuthorizationServiceFactory : IHubspotAuthorizationServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public HubspotAuthorizationServiceFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public IHubspotAuthorizationService GetAuthorizationService(bool useUmbracoAuthorization) =>
        useUmbracoAuthorization
            ? _serviceProvider.GetRequiredService<UmbracoAuthorizationService>()
            : _serviceProvider.GetRequiredService<AuthorizationService>();

}
