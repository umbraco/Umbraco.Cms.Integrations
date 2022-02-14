using Umbraco.Cms.Integrations.Shared.Configuration;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Cms.Integrations.Shared
{
    public class Dependencies: IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IAppSettings, AppSettingsWrapper>(Lifetime.Singleton);
        }
    }
}
