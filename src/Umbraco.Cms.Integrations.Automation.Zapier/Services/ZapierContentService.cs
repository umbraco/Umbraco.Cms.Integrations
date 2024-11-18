using System;
using System.Collections.Generic;
using System.Linq;


#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapierContentService : IZapierContentService
    {
        private readonly IZapierContentFactory _zapierContentFactory;

        public ZapierContentService(IZapierContentFactory zapierContentFactory) =>
            _zapierContentFactory = zapierContentFactory;

        public Dictionary<string, string> GetContentTypeDictionary(IContentType contentType, IPublishedContent content)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.ContentProperties.Id, content != null ? content.Id.ToString() : "1" },
                {Constants.ContentProperties.Name, content != null ? content.Name : contentType.Name },
                {Constants.ContentProperties.PublishDate, content != null ? content.UpdateDate.ToString("s") : DateTime.UtcNow.ToString("s") }
            };

            foreach (var propertyType in contentType.PropertyTypes)
            {
                if (content == null)
                {
                    contentDict.Add(propertyType.Alias, string.Empty);
                    continue;
                }

                var contentProperty = content.Properties.First(p => p.Alias == propertyType.Alias);

                var parser = _zapierContentFactory.Create(contentProperty.PropertyType.EditorAlias);

                if (contentDict.ContainsKey(propertyType.Alias))
                {
                    continue;
                }

                contentDict.Add(propertyType.Alias, parser.GetValue(contentProperty));
            }

            return contentDict;
        }

        public Dictionary<string, string> GetContentDictionary(IContent contentNode)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.ContentProperties.Id, contentNode.Id.ToString() },
                {Constants.ContentProperties.Name, contentNode.Name },
                {Constants.ContentProperties.PublishDate, contentNode.UpdateDate.ToString("s") }
            };

            foreach (var prop in contentNode.Properties)
            {
                if (contentDict.ContainsKey(prop.Alias))
                {
                    continue;
                }

                contentDict.Add(prop.Alias, prop.Id == 0 || prop.Values.Count == 0 ? string.Empty : prop.GetValue().ToString());
            }

            return contentDict;
        }
    }
}
