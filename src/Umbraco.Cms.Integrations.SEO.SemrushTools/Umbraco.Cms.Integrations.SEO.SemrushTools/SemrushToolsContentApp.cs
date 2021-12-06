using System.Collections.Generic;

using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools
{
    public class SemrushToolsContentApp : IContentAppFactory
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
                Alias = "semrushTools",
                Name = "Semrush Tools",
                Icon = "icon-files",
                View = "/App_Plugins/UmbracoCms.Integrations/SEO/SemrushTools/semrushTools.html",
                Weight = 1
            };
        }
    }
}
