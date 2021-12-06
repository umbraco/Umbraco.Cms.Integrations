using Umbraco.Core.Composing;
using Umbraco.Web;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools
{
    public class SemrushToolsComposer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.ContentApps().Append<SemrushToolsContentApp>();
        }
    }
}
