using Umbraco.Cms.Integrations.SEO.Semrush.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;

namespace Umbraco.Cms.Integrations.SEO.Semrush
{
    public class SemrushComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.ContentApps().Append<SemrushContentApp>();

            composition.Components().Append<SemrushComponent>();

            composition.Register<ISemrushTokenService, SemrushTokenService>(Lifetime.Singleton);
            composition.Register<ICacheHelper, CacheHelper>(Lifetime.Singleton);

            composition.Register<TokenBuilder>(Lifetime.Request);
        }
    }
}
