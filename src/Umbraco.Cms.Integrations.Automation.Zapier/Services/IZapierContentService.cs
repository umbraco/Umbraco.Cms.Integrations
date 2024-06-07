using System.Collections.Generic;

#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models;

#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public interface IZapierContentService
    {
        Dictionary<string, string> GetContentTypeDictionary(IContentType contentType, IPublishedContent content);

        Dictionary<string, string> GetContentDictionary(IContent contentNode);
    }
}
