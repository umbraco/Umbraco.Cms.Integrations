namespace Umbraco.Cms.Integrations.Crm.Hubspot.Services;

public interface IHubspotAuthorizationServiceFactory
{
    IHubspotAuthorizationService GetAuthorizationService(bool useUmbracoAuthorization);
}
