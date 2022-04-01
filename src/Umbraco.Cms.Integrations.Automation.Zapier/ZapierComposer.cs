using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Cms.Integrations.Automation.Zapier
{
    public class ZapierComposer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ZapConfigService>(Lifetime.Singleton);

            composition.Register<ZapierService>(Lifetime.Singleton);
        }
    }
}
