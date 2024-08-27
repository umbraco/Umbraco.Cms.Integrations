using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public interface IZapierContentService
    {
        Dictionary<string, string> GetContentTypeDictionary(IContentType contentType, IPublishedContent content);

        Dictionary<string, string> GetContentDictionary(IContent contentNode);
    }
}
