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

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool
{
    public class URLInspectionToolContentApp : IContentAppFactory
    {
        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            if (!(source is IContent)) return null;

            var content = (IContent) source;

            if (content.Id == 0) return null;

            return new ContentApp
            {
                Alias = "urlInspectionTool",
                Name = "URL Inspection",
                Icon = "icon-search",
                View = "/App_Plugins/UmbracoCms.Integrations/SEO/GoogleSearchConsole/URLInspectionTool/urlInspection.html",
                Weight = 1
            };
        }
    }
}
