using System.Collections.Generic;

#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
#else
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;
#endif

namespace Umbraco.Cms.Integrations.SEO.Semrush
{
    public class SemrushContentApp : IContentAppFactory
    {
        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            if (!(source is IContent))
                return null;

            var content = (IContent)source;

            if (content.Id == 0)
                return null;

            return new ContentApp
            {
                Alias = "semrush",
                Name = "Semrush",
                Icon = "icon-files",
                View = "/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrush.html",
                Weight = 1
            };
        }
    }
}
