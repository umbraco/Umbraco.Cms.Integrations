using Umbraco.Cms.Integrations.Shared.Configuration;
using Umbraco.Cms.Integrations.Shared.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Cms.Integrations.Shared
{
    public class Dependencies: IComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IAppSettings, AppSettingsWrapper>(Lifetime.Singleton);

            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);

            composition.Register<ICacheHelper, CacheHelper>(Lifetime.Singleton);
        }
    }
}
