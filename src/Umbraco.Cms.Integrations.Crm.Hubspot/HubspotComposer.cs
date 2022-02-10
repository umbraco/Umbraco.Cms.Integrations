using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    public class HubspotComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IAppSettings, AppSettingsWrapper>(Lifetime.Singleton);

            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);
        }
    }
}
