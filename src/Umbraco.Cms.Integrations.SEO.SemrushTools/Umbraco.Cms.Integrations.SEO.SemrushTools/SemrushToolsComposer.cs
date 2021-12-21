using Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.SemrushTools.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools
{
    public class SemrushToolsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.ContentApps().Append<SemrushToolsContentApp>();

            composition.Components().Append<SemrushToolsComponent>();

            composition.Register<ISemrushService<TokenDto>, SemrushService>(Lifetime.Singleton);
            composition.Register<ISemrushCachingService<RelatedPhrasesDto>, SemrushCachingService>(Lifetime.Singleton);
        }
    }
}
