using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.NotificationHandlers;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Services;

#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#else
using Umbraco.Core.Composing;
using Umbraco.Core;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools
{
    public class CommerceToolsComposer : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddCommerceTools();
        }
#else

        public void Compose(Composition composition)
        {
            composition.Register<ICommerceToolsService, CommerceToolsService>(Lifetime.Singleton);

            composition.Components().Append<AddApiBaseUrl>();
        }
#endif
    }
}
