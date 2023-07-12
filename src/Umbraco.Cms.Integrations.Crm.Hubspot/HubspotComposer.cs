using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    public class HubspotComposer : IComposer
    {
        public delegate IHubspotAuthorizationService AuthorizationImplementationFactory(bool useUmbracoAuthorization);

        public void Compose(Composition composition)
        {
            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);

            composition.Register<UmbracoAuthorizationService>(Lifetime.Singleton);
            composition.Register<AuthorizationService>(Lifetime.Singleton);
            composition.Register<AuthorizationImplementationFactory>(f => (useUmbracoAuthorization) =>
            {
                if (useUmbracoAuthorization)
                    return f.GetInstance<UmbracoAuthorizationService>();

                return f.GetInstance<AuthorizationService>();
            }, Lifetime.Singleton);
        }
    }
}
