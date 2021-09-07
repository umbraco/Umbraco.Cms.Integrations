using Umbraco.Cms.Integrations.Commerce.CommerceTools.NotificationHandlers;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools
{
    public class CommerceToolsComposer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ICommerceToolsService, CommerceToolsService>(Lifetime.Singleton);

            composition.Components().Append<AddApiBaseUrl>();
        }
    }
}
